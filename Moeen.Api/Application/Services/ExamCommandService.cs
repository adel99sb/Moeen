using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ExamCommand;

namespace Moeen.Api.Application.Services
{
   public class ExamCommandService : IExamCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        // ✅ Constructor Dependency Injection
        public ExamCommandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResponse> AddExamFeedbackAsync(AddExamFeedbackRequest request)
        {
            try
            {
                // 1. التحقق من الطلب
                if (request == null)
                    return GeneralResponse.BadRequest("طلب البيانات مطلوب.");

                // 2. جلب الامتحان والتأكد من وجوده
                var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId);
                if (exam == null)
                    return GeneralResponse.BadRequest("الامتحان غير موجود.");

                // 3. تحديث البيانات
                // ⚠️ ملاحظة معمارية: حقل Feedback يُخزن في عمود notes حالياً حسب الـ Schema الموجود لديك
                exam.notes = request.Feedback;

                // 4. الحفظ في قاعدة البيانات
                await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
                await _unitOfWork.CompleteAsync();

                // 5. بناء الـ DTO المطلوب
                var dto = new ExamFeedbackDto
                {
                    ExamId = exam.Id,
                    Feedback = exam.notes,
                    Recommendations = request.Recommendations,
                    UpdatedAt = DateTime.UtcNow
                };

                // 6. إرجاع الاستجابة الموحدة
                return GeneralResponse.Ok("تم إضافة ملاحظات وتوصيات الامتحان بنجاح.", dto);
            }
            catch (Exception)
            {
                // ✅ ضمان وجود return في جميع المسارات (يحل مشكلة CS0161 نهائياً)
                return GeneralResponse.InternalError("حدث خطأ داخلي أثناء إضافة ملاحظات الامتحان.");
            }
        }
        public async Task<GeneralResponse> DeleteExamResultAsync(DeleteExamResultRequest request)
        {
            try
            {
                // 1. التحقق من الطلب
                if (request == null)
                    return GeneralResponse.BadRequest("طلب البيانات مطلوب.");

                // 2. جلب الامتحان والتأكد من وجوده
                var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId);
                if (exam == null)
                    return GeneralResponse.BadRequest("الامتحان غير موجود.");

                // 3. حذف السجل
                await _unitOfWork.Repository<Exam>().DeleteAsync(exam);
                await _unitOfWork.CompleteAsync();

                // 4. بناء كائن الاستجابة الخاص (اختياري، يمكن الاستغناء عنه والاعتماد على GeneralResponse مباشرة)
                var resultDto = new DeleteExamResultResponse
                {
                    Success = true,
                    Message = "تم حذف نتيجة الامتحان بنجاح."
                };

                // 5. إرجاع الاستجابة الموحدة
                return GeneralResponse.Ok("تم حذف نتيجة الامتحان بنجاح.", resultDto);
            }
            catch (Exception)
            {
                // ⚠️ يفضل تسجيل الخطأ هنا بـ ILogger في البيئة الحقيقية
                return GeneralResponse.InternalError("حدث خطأ داخلي أثناء حذف نتيجة الامتحان.");
            }
        }

        public async Task<GeneralResponse> RegisterExamAsync(RegisterExamRequest request)
        {
            try
            {
                // 1. التحقق من الطلب
                if (request == null)
                    return GeneralResponse.BadRequest("طلب البيانات مطلوب.");

                // 2. قواعد أعمال (Business Rules)
                if (request.JuzTo < request.JuzFrom)
                    return GeneralResponse.BadRequest("الجزء 'إلى' يجب أن يكون أكبر من أو يساوي الجزء 'من'.");

                // 3. التحقق من وجود الطالب والمعلم (منع أخطاء Foreign Key وجلب الأسماء)
                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(request.StudentId);
                if (student == null)
                    return GeneralResponse.BadRequest("الطالب المحدد غير موجود.");

                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
                if (teacher == null)
                    return GeneralResponse.BadRequest("المعلم المحدد غير موجود.");

                if (teacher.status != 0)
                    return GeneralResponse.BadRequest("لا يمكن تسجيل اختبار على معلم غير نشط.");

                // 4. إنشاء كيان الامتحان وحفظه
                var exam = new Exam
                {
                    Id = Guid.NewGuid(),
                    StudentId = request.StudentId,
                    TeacherId = request.TeacherId,
                    juz_form = request.JuzFrom,       // مطابقة لأسماء الحقول في الـ Entity
                    juz_to = request.JuzTo,
                    score = request.Score,
                    notes = request.Notes ?? string.Empty,
                    mark = request.Mark,
                    date = DateTime.UtcNow,
                    TeacherExamId = Guid.NewGuid()    // يُنشأ تلقائياً إذا كان الجدول يتطلبه
                };

                await _unitOfWork.Repository<Exam>().AddAsync(exam);
                await _unitOfWork.CompleteAsync();

                // 5. بناء الـ DTO وإرجاع الاستجابة الموحدة
                var dto = new ExamResultDto
                {
                    Id = exam.Id,
                    StudentId = exam.StudentId,
                    StudentName = student.name,       // افتراض أن خاصية الاسم هي 'name' في الـ User/Student
                    TeacherId = exam.TeacherId,
                    TeacherName = teacher.name,
                    HalqaTeacherName = string.Empty,
                    ExaminerName = teacher.name,
                    JuzFrom = exam.juz_form,
                    JuzTo = exam.juz_to,
                    Score = exam.score,
                    Date = exam.date,
                    Notes = exam.notes,
                    Mark = exam.mark,
                    Grade = CalculateGrade(exam.score) // دالة مساعدة للتقدير
                };

                return GeneralResponse.Ok("تم تسجيل الاختبار بنجاح.", dto);
            }
            catch (Exception)
            {
                // ⚠️ يُفضل تسجيل الـ ex باستخدام ILogger في الإنتاج
                return GeneralResponse.InternalError("حدث خطأ داخلي أثناء تسجيل الاختبار.");
            }
        }

        // 🛠️ دالة مساعدة داخل الخدمة لتحويل العلامة التقديرية
        private string CalculateGrade(int score)
        {
            if (score >= 90) return "ممتاز";
            if (score >= 80) return "جيد جداً";
            if (score >= 70) return "جيد";
            if (score >= 60) return "مقبول";
            return "يحتاج تحسين";
        }

        public async Task<GeneralResponse> UpdateExamInfoAsync(UpdateExamInfoRequest request)
        {
            try
            {
                // 1. التحقق من الطلب
                if (request == null)
                    return GeneralResponse.BadRequest("طلب البيانات مطلوب.");

                // 2. جلب الامتحان
                var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId);
                if (exam == null)
                    return GeneralResponse.BadRequest("الامتحان غير موجود.");

                // 3. تحديث الحقول الموجودة فعلياً في الـ Schema فقط
                if (request.ExamDate.HasValue)
                    exam.date = request.ExamDate.Value;

                if (!string.IsNullOrWhiteSpace(request.Notes))
                    exam.notes = request.Notes;

                // 4. الحفظ
                await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
                await _unitOfWork.CompleteAsync();

                // 5. جلب البيانات المرتبطة لبناء الـ DTO
                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(exam.StudentId);
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(exam.TeacherId);

                // 6. بناء الاستجابة الموحدة
                var dto = new ExamResultDto
                {
                    Id = exam.Id,
                    StudentId = exam.StudentId,
                    StudentName = student?.name ?? "غير معروف",
                    TeacherId = exam.TeacherId,
                    TeacherName = teacher?.name ?? "غير معروف",
                    HalqaTeacherName = string.Empty,
                    ExaminerName = teacher?.name ?? "غير معروف",
                    JuzFrom = exam.juz_form,
                    JuzTo = exam.juz_to,
                    Score = exam.score,
                    Date = exam.date,
                    Notes = exam.notes,
                    Mark = exam.mark,
                    Grade = CalculateGrade(exam.score) // الدالة المساعدة من الخطوة السابقة
                };

                return GeneralResponse.Ok("تم تحديث بيانات الامتحان بنجاح.", dto);
            }
            catch (Exception)
            {
                return GeneralResponse.InternalError("حدث خطأ داخلي أثناء تحديث بيانات الامتحان.");
            }
        }

        public async Task<GeneralResponse> UpdateExamResultAsync(UpdateExamResultRequest request)
        {
            try
            {
                // 1. التحقق من الطلب
                if (request == null)
                    return GeneralResponse.BadRequest("طلب البيانات مطلوب.");

                // 2. جلب الامتحان والتأكد من وجوده
                var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId);
                if (exam == null)
                    return GeneralResponse.BadRequest("الامتحان غير موجود.");

                // 3. تحديث النتيجة والملاحظات فقط
                exam.score = request.NewScore;
                if (!string.IsNullOrWhiteSpace(request.Notes))
                    exam.notes = request.Notes;

                // ️ ExamType غير موجود في قاعدة البيانات، يُستخدم للمنطق البرمجي فقط ولا يُحفظ هنا

                // 4. الحفظ
                await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
                await _unitOfWork.CompleteAsync();

                // 5. جلب بيانات الطالب والمعلم لبناء الـ DTO
                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(exam.StudentId);
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(exam.TeacherId);

                // 6. بناء الـ DTO وإرجاع الاستجابة الموحدة
                var dto = new ExamResultDto
                {
                    Id = exam.Id,
                    StudentId = exam.StudentId,
                    StudentName = student?.name ?? "غير معروف",
                    TeacherId = exam.TeacherId,
                    TeacherName = teacher?.name ?? "غير معروف",
                    JuzFrom = exam.juz_form,
                    JuzTo = exam.juz_to,
                    Score = exam.score,
                    Date = exam.date,
                    Notes = exam.notes,
                    Mark = exam.mark,
                    Grade = CalculateGrade(exam.score) // الدالة المساعدة
                };

                return GeneralResponse.Ok("تم تحديث نتيجة الامتحان بنجاح.", dto);
            }
            catch (Exception)
            {
                return GeneralResponse.InternalError("حدث خطأ داخلي أثناء تحديث نتيجة الامتحان.");
            }
        }

        public async Task<GeneralResponse> CreateLabExamAsync(CreateLabExamRequest request)
        {
            try
            {
                if (request == null)
                    return GeneralResponse.BadRequest("بيانات الاختبار مطلوبة.");

                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(request.StudentId);
                if (student == null) return GeneralResponse.NotFound("الطالب غير موجود.");

                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
                if (teacher == null) return GeneralResponse.NotFound("المعلم غير موجود.");

                if (teacher.status != 0)
                    return GeneralResponse.BadRequest("لا يمكن تسجيل اختبار على معلم غير نشط.");

                // دمج الملاحظات مع التقييم واسم الفوج لعدم وجود حقول مستقلة لها في الكيان حالياً
                var enrichedNotes = $"[التقييم: {request.Rating}] [الفوج: {request.FoujName}] {request.Notes}";

                var exam = new Exam
                {
                    Id = Guid.NewGuid(),
                    StudentId = request.StudentId,
                    TeacherId = request.TeacherId,
                    date = request.ExamDate == default ? DateTime.UtcNow : request.ExamDate,
                    score = request.Grade,      // تخزين الدرجة في Score
                    mark = request.PointsAwarded, // تخزين النقاط في Mark
                    notes = enrichedNotes,
                    juz_form = 0, // افتراضي حيث لم يحدد في طلب المختبر
                    juz_to = 0,
                    TeacherExamId = Guid.Empty // أو معرف افتراضي
                };

                await _unitOfWork.Repository<Exam>().AddAsync(exam);
                
                // تحديث نقاط الطالب في جدول الطلاب
                student.score += request.PointsAwarded;
                await _unitOfWork.Repository<Student>().UpdateAsync(student);

                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("تم تسجيل اختبار المختبر بنجاح.");
            }
            catch (Exception)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء تسجيل اختبار المختبر.");
            }
        }
    }
}
