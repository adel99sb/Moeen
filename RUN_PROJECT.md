# تشغيل مشروع Moeen للرفقات

هذا الملف مخصص للتشغيل المحلي بدون الدخول بتفاصيل تقنية كثيرة.

## المتطلبات

لازم يكون مثبت على الجهاز:

1. .NET SDK مناسب للمشروع.
2. SQL Server LocalDB أو SQL Server Express.
3. Git.
4. لتشغيل الموبايل: Visual Studio/MAUI workload و Android device أو emulator.

## أول تشغيل

افتح PowerShell من مجلد المشروع الأساسي، ثم شغّل الـ API أولاً:

```powershell
.\scripts\run-api.ps1
```

اترك نافذة الـ API مفتوحة. أول تشغيل ممكن يطول قليلاً لأن المشروع سيعمل تلقائياً في Development على:

- تطبيق EF migrations على قاعدة البيانات.
- إنشاء الأدوار Roles.
- إنشاء بيانات seed التجريبية إذا لم تكن موجودة.

بعد نجاح الـ API شغّل Dashboard بنافذة PowerShell ثانية:

```powershell
.\scripts\run-dashboard.ps1
```

الروابط الافتراضية:

- API Swagger: `https://localhost:7023/swagger`
- API Health: `http://localhost:5055/health`
- Dashboard: `https://localhost:7220`

## حسابات تجريبية

كلمة المرور لكل الحسابات:

```text
MoeenDevOnly123!
```

الحسابات:

```text
owner@moeen.local
supervisor@moeen.local
teacher@moeen.local
examer@moeen.local
student@moeen.local
parent@moeen.local
```

## فحص المشاكل بسرعة

شغّل:

```powershell
.\scripts\check-dev.ps1
```

هذا يفحص:

- إصدار dotnet.
- وجود LocalDB.
- هل ports مستخدمة.
- هل API health شغال.

## مشاكل شائعة وحلها

### 1. API لا يشتغل بسبب قاعدة البيانات

راجع رسائل الـ console في نافذة API. صار عند التشغيل يظهر log واضح باسم:

```text
Startup.Database
```

إذا ظهر فشل عند migrations غالباً السبب واحد من التالي:

- SQL Server LocalDB غير مثبت.
- SQL Server غير شغال.
- connection string غير صحيح في `Moeen.Api/appsettings.json`.
- المستخدم الحالي لا يملك صلاحية إنشاء قاعدة بيانات.

الـ connection string الافتراضي:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MoeenDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 2. Port مستخدم

الـ API يستخدم:

```text
5055 http
7023 https
```

الـ Dashboard يستخدم:

```text
5077 http
7220 https
```

لمعرفة من يستخدم port:

```powershell
.\scripts\check-dev.ps1
```

أو أغلق المشروع القديم إذا كان لا يزال يعمل.

### 3. Dashboard لا يتصل بالـ API

الافتراضي أن Dashboard يتصل على:

```text
https://localhost:7023/
```

إذا غيّرت API port، شغّل Dashboard مع متغير:

```powershell
$env:MOEEN_API_BASE_URL="http://localhost:5055/"
.\scripts\run-dashboard.ps1
```

### 4. الموبايل لا يتصل بالـ API

داخل `Moeen.App/Infrastructure/Http/ApiRouts.cs` يوجد Android URL:

```csharp
http://192.168.1.100:5055/
```

إذا الهاتف الحقيقي على نفس الشبكة، غيّر `192.168.1.100` إلى IP جهاز اللابتوب الذي يشغل الـ API.

لمعرفة IP الجهاز:

```powershell
ipconfig
```

استخدم IPv4 الخاص بالـ Wi-Fi.

## تشغيل الموبايل على جهاز Android متصل

قبل تشغيل الموبايل، شغّل الـ API أولاً واترك نافذته مفتوحة:

```powershell
.\scripts\run-api.ps1
```

بعدها وصل الموبايل USB وفعل Developer Options + USB debugging، ثم شغّل:

```powershell
.\scripts\run-mobile.ps1
```

السكريبت يقوم تلقائياً بـ:

- فحص dotnet، وإذا كان ناقصاً يحاول تثبيت .NET SDK 10 عبر winget.
- فحص winget، وإذا كان ناقصاً يحاول تثبيت Microsoft App Installer من الرابط الرسمي `https://aka.ms/getwinget`.
- فحص Java، وإذا كانت ناقصة يحاول تثبيت Microsoft OpenJDK 17 عبر winget.
- فحص MAUI/Android workload، وإذا ناقصة يحاول تثبيتها عبر dotnet workload.
- البحث أولاً عن Android SDK موجود من `ANDROID_HOME` أو `ANDROID_SDK_ROOT` أو المسارات المعروفة.
- إذا Android SDK موجود يستخدمه ولا ينزله من جديد.
- إذا Android SDK موجود لكن ناقصه command-line tools أو packages، يثبت الناقص فقط.
- إذا لا يوجد Android SDK أبداً، ينشئ واحداً محلياً خارج Visual Studio تحت `%LOCALAPPDATA%\Android\Sdk`.
- تنزيل Android command-line tools مباشرة من Google عند الحاجة.
- تثبيت Android SDK packages الناقصة: `platform-tools`, `platforms;android-36`, `build-tools;35.0.0`.
- قبول Android SDK licenses.
- ضبط `ANDROID_HOME` و `ANDROID_SDK_ROOT` للمستخدم الحالي.
- فحص وجود جهاز Android متصل.
- فحص API health.
- تشغيل `adb reverse tcp:5055 tcp:5055` حتى يرى الموبايل الـ API على `http://127.0.0.1:5055/`.
- بناء APK ثابت.
- تثبيت أو تحديث التطبيق.
- تشغيل التطبيق على الموبايل.

إذا كان أكثر من موبايل متصل، استخدم:

```powershell
adb devices
.\scripts\run-mobile.ps1 -DeviceId DEVICE_ID
```

ملاحظة: الموبايل حالياً مضبوط في `Moeen.App/Infrastructure/Http/ApiRouts.cs` لاستخدام `http://127.0.0.1:5055/` على Android، وهذا يعتمد على `adb reverse`. لذلك لا تحتاج لتغيير IP عند استخدام USB.

## ملاحظات مهمة

- لا تحتاج لتطبيق migrations يدوياً في Development؛ الـ API يطبقها عند التشغيل.
- seed لا ينشئ نسخ مكررة لنفس الحسابات التجريبية، لأنه يفحص وجودها أولاً.
- في حال حدث خطأ تشغيل، انسخ آخر 30 سطر من نافذة API أو Dashboard أو run-mobile وأرسلها للمطور.
