
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.Shared.Requests.Exam_Halqa;
using Moeen.Api.Shared.Responses.Exam_Halqa;
using Moeen.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
    {
        public class ExamHalqaService : IExamHalqaService
        {
            private readonly IUnitOfWork _unitOfWork;

            public ExamHalqaService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<GeneralResponse> CreateAsync(CreateExamTeacherRequest request)
            {
                try
                {
                    // 1. التحقق من الطلب
                    if (request?.ExamTeacherData == null)
                        return GeneralResponse.BadRequest("بيانات أستاذ الاختبارات مطلوبة.");

                    var data = request.ExamTeacherData;

                    // 2. التحقق من عدم وجود أستاذ اختبارات بنفس المعرف مسبقاً
                    var existingTeacher = await _unitOfWork.Repository<TeacherExam>()
                        .GetByIdAsync(data.TeacherExamId);

                    if (existingTeacher != null)
                        return GeneralResponse.BadRequest("أستاذ الاختبارات مسجل مسبقاً.");

                    // 3. التحقق من وجود المسجد
                    var mosque = await _unitOfWork.Repository<Mosque>()
                        .GetByIdAsync(data.MosqueId);
                    if (mosque == null)
                        return GeneralResponse.BadRequest("المسجد المحدد غير موجود.");

                    // 4. إنشاء أستاذ الاختبارات الجديد
                    var newTeacherExam = new TeacherExam
                    {
                        Id = data.TeacherExamId,
                        Name = data.Name,
                        MosquId = data.MosquId,
                        Bio = data.Bio,
                        CreatedAt = DateTime.UtcNow,
                        // إذا TeacherExam بيرث من User، ممكن تحتاج تضيف خصائص User
                        // مثل: Email, PhoneNumber, إلخ
                    };

                    await _unitOfWork.Repository<TeacherExam>().AddAsync(newTeacherExam);
                    await _unitOfWork.CompleteAsync();

                    // 5. بناء الـ DTO للرد
                    var dto = new ExamTeacherResponse
                    {
                        Id = newTeacherExam.Id,
                        TeacherName = newTeacherExam.Name,
                        MosqueId = newTeacherExam.MosquId,
                        Bio = newTeacherExam.Bio
                    };

                    var responseDto = new CreateExamTeacherResponse
                    {
                        Success = true,
                        Message = "تم تسجيل أستاذ الاختبارات بنجاح",
                        ExamTeacher = dto
                    };

                    return GeneralResponse.Ok("تم تسجيل أستاذ الاختبارات بنجاح.", responseDto);
                }
                catch (Exception)
                {
                    return GeneralResponse.InternalError("حدث خطأ داخلي أثناء تسجيل أستاذ الاختبارات.");
                }
            }

            public async Task<GeneralResponse> GetByIdAsync(GetExamTeacherByIdRequest request)
            {
                try
                {
                    // 1. التحقق من الطلب
                    if (request == null || request.Id == Guid.Empty)
                        return GeneralResponse.BadRequest("معرف أستاذ الاختبارات مطلوب.");

                    // 2. جلب أستاذ الاختبارات مع حلقاته
                    var teacherExam = await _unitOfWork.Repository<TeacherExam>()
                        .GetByIdAsync(request.Id);

                    if (teacherExam == null)
                        return GeneralResponse.NotFound("أستاذ الاختبارات غير موجود.");

                    // 3. تحميل الحلقات المرتبطة (من جدول ExamTeacherHalqa)
                    var halqas = (await _unitOfWork.Repository<ExamTeacherHalqa>().GetAllAsync())
                        .Where(eth => eth.ExamTeacherId == teacherExam.Id)
                        .Select(eth => new {
                            eth.HalqaId,
                            eth.FoujId
                        })
                        .ToList();

                    // 4. بناء الـ DTO
                    var dto = new ExamTeacherResponse
                    {
                        Id = teacherExam.Id,
                        TeacherName = teacherExam.Name,
                        MosqueId = teacherExam.MosquId,
                        Bio = teacherExam.Bio,
                        HalqasCount = halqas.Count
                    };

                    var responseDto = new GetExamTeacherByIdResponse
                    {
                        Success = true,
                        Message = "تم جلب البيانات بنجاح",
                        ExamTeacher = dto
                    };

                    return GeneralResponse.Ok("تم جلب بيانات أستاذ الاختبارات بنجاح.", responseDto);
                }
                catch (Exception)
                {
                    return GeneralResponse.InternalError("حدث خطأ داخلي أثناء جلب البيانات.");
                }
            }

            public async Task<GeneralResponse> AssignHalqaAsync(AssignHalqaToExamTeacherRequest request)
            {
                try
                {
                    // 1. التحقق من صحة الطلب
                    if (request == null || request.ExamTeacherId == Guid.Empty || request.HalqaId == Guid.Empty)
                        return GeneralResponse.BadRequest("بيانات التعيين غير مكتملة.");

                    // 2. التحقق من وجود أستاذ الاختبارات
                    var teacherExam = await _unitOfWork.Repository<TeacherExam>()
                        .GetByIdAsync(request.ExamTeacherId);
                    if (teacherExam == null)
                        return GeneralResponse.BadRequest("أستاذ الاختبارات غير موجود.");

                    // 3. التحقق من وجود الحلقة
                    var halqa = await _unitOfWork.Repository<Halqa>()
                        .GetByIdAsync(request.HalqaId);
                    if (halqa == null)
                        return GeneralResponse.BadRequest("الحلقة المحددة غير موجودة.");

                    // 4. التحقق من وجود الفوج (إذا تم تمريره)
                    if (request.FoujId != Guid.Empty)
                    {
                        var fouj = await _unitOfWork.Repository<Fouj>()
                            .GetByIdAsync(request.FoujId);
                        if (fouj == null)
                            return GeneralResponse.BadRequest("الفوج المحدد غير موجود.");
                    }

                    // 5. التحقق من عدم وجود ربط مسبق في جدول ExamTeacherHalqa
                    var repo = _unitOfWork.Repository<ExamTeacherHalqa>();
                    var allLinks = await repo.GetAllAsync();

                    bool isDuplicate = allLinks.Any(link =>
                        link.ExamTeacherId == request.ExamTeacherId &&
                        link.HalqaId == request.HalqaId &&
                        (request.FoujId == Guid.Empty || link.FoujId == request.FoujId));

                    if (isDuplicate)
                        return GeneralResponse.BadRequest("هذا الربط موجود مسبقاً في النظام.");

                    // 6. إنشاء سجل الربط الجديد (Junction Record)
                    var newLink = new ExamTeacherHalqa
                    {
                        Id = Guid.NewGuid(),
                        ExamTeacherId = request.ExamTeacherId,
                        HalqaId = request.HalqaId,
                        FoujId = request.FoujId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await repo.AddAsync(newLink);
                    await _unitOfWork.CompleteAsync();

                    // 7. إرجاع الاستجابة الموحدة
                    return GeneralResponse.Ok("تم تعيين الحلقة لأستاذ الاختبارات بنجاح.");
                }
                catch (Exception)
                {
                    return GeneralResponse.InternalError("حدث خطأ داخلي أثناء عملية التعيين.");
                }
            }
        internal class ExamTeacher
        {
            public Guid Id { get; set; }
            public Guid TeacherId { get; set; }
            public Guid HalqaId { get; set; }
            public Guid ExamTypeId { get; set; }
            public DateTime AssignedDate { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
    }