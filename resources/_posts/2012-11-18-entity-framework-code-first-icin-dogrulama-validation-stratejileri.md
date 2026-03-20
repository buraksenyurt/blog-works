---
layout: post
title: "Entity Framework Code First için Doğrulama(Validation) Stratejileri"
date: 2012-11-18 06:38:00 +0300
categories:
  - entity-framework
tags:
  - entity-framework
  - csharp
  - dotnet
  - aspnet
  - linq
  - http
  - authentication
  - generics
  - webinar
---
Bir verinin çeşitli kurallara göre doğrulanması, verinin işlenmek üzere gönderilmeden önce yapılması gereken önemli işlemlerden birisidir. Özellikle Entity Framework gibi veri merkezli (Data-Centric) uygulama geliştirme alt yapılarında bu durum daha da önem arz etmektedir. Burada söz konusu olan, görsel bir kontrolün içerik denetiminden ziyade, çalışma zamanı Entity örneklerine ait özelliklerin (Property) değerlerinin denetlenmesidir. Çok doğal olarak verilerde tutarsızlıklara neden olabilecek çeşitli ihlallerin tespit edilmesi, toplanması, gerektiğinde son kullanıcıya bildirilmesi ya da farklı bir yere raporlanması/loglanması gerekmektedir.

[![checklist1](/assets/images/2012/checklist1_thumb.gif)](/assets/images/2012/checklist1.gif)

Peki verinin doğrulanmasından tam olarak beklentilerimiz neler olabilir? Bunu bir kaç gerçek hayat ihtiyacı ile cevaplayabiliriz.

Örneğin,

- Kullanıcı isminin en az 5, en fazla 25 karakter olması istenebilir.
- Oyuncunun kullanmak istediği dil sadece ingilizce, almanca ve fransızca olsun, şeklinde bir zorlama yapılması söz konusu olabilir.
- Doğum tarihinin bu günün ötesinde olmaması gerekebilir.
- Girilen URL adresinin istenen formatta olması beklenebilir.
- Yazarın vermiş olduğu sosyal güvenlik numarasının gerçekten de var olmaması bir doğrulama ihlali olarak düşünülebilir.
- vb...

Örnekler duruma göre çoğaltılabilir elbette. Entity Framework, özelliklerin doğrulanması için bir kaç noktada araya girmemizi sağlayacak imkanlar sunmaktadır. Biz bu örneğimizde Code-First yaklaşımı üzerinden ilgili doğrulama kurallarını hangi noktalardan ve nasıl enjekte edebileceğimizi incelemeye çalışıyor olacağız. Doğrulama işlemlerini temelde Entity ve Context seviyesinde olmak üzere iki ana dala ayırabiliriz.

Doğrulama kuralları (Validation Rules), Entity seviyesinde iki şekilde yaptırılabilir. Nitelik (Attribute) bazlı veya IValidatableObject arayüzünün implementasyonu ile. Nitelik bazlı enjektelerde, System.ComponentModel.DataAnnotations isim alanı (namespace) altında yer alan bazı nitelik tiplerinden yararlanılır.

Context seviyesinde ise ValidateEntity sanal metodunun ezilmesi (override) suretiyle söz konusu denetimler yaptırılabilir.

Hangi teknik seçilirse seçilsin, ilgili doğrulama kurallarının geçersiz olması halinde, ortama fırlatılan Exception'ların da kümülatif olarak toplanıp sunulması önemlidir. Bilindiği üzere normal şartlarda, kod akarken oluşan bir Exception sonucu çalışma zamanı catch bloğuna atlayacak ve try bloğu içerisindeki akışına devam etmeyecektir. Entity Framework gibi kullanım alanlarında, birden fazla Entity söz konusu olmakla birlikte bunların herhangibirisine ait özelliklerde oluşacak olan doğrulama ihlallerin toplanıp sunulması çok daha doğru bir yaklaşım olacaktır.

Entity Framework bu noktada bize yardımcı olacak bir Exception tipi içerir; DbEntityValidationException. Bu exception tipine ait EntityValidationErros koleksiyonu, Entity'ler için söz konusu olabilecek tüm doğrulama ihlallerini bünyesinde toplamaktadır. Bu sayede kullanıcıya toplu bir hata listesini döndürmemiz mümkün olabilir.

Şimdi dilerseniz basit bir Console uygulaması üzerinden söz konusu teknikleri irdelemeye çalışalım.

> Code-First yaklaşımını tercih ettiğimiz bu örnekte NuGet yardımıyla ilgili Entity Framework paketinin yüklenmiş olduğunu varsayıyoruz. Örneği Entity Framework'ün 4.5 sürümü üzerinde geliştirmekteyiz.

İlk olarak aşağıdaki sınıf çizelgesinde yer alan Layer POCO tipini geliştirdiğimizi düşünelim.

[![efv_1](/assets/images/2012/efv_1_thumb.png)](/assets/images/2012/efv_1.png)

```csharp
using System.ComponentModel.DataAnnotations;

namespace HowTo_Validation 
{ 
    public class Layer 
    { 
        public int LayerId { get; set; } 
        
        [MaxLength(25,ErrorMessage="Katman başlığı en fazla 25 karakter olabilir") 
        , Required(ErrorMessage="Bir başlık girilmelidir")] 
        public string Title { get; set; } 
        
       [Required(ErrorMessage="Maximum oyuncu kapasitesini girmelisiniz") 
        , Range(5,100,ErrorMessage="En az 5 en fazla 100 oyuncu olabilir")] 
        public int MaxPlayerCapacity { get; set; }

        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Geçersiz posta adresi")] 
        public string ModeratorMail { get; set; } 
    } 
}
```

Layer tipi içerisinde yer alan Title, MaxPlayerCapacity ve ModeratorMail özelliklerine çeşitli nitelikler uygulandığı görülmektedir. Bu niteliklerin ErrorMessage kısmında genellikle ihlallere ilişkin bir mesaj kullanılır. MaxLength, tahmin edileceği üzere ilgili özelliğin maksimum karakter sayısını ifade etmektedir. Diğer yandan Range niteliği ile sayısal bir özellik için gerekli olan alt ve üst sınır değerleri belirtilmektedir.

Required niteliği, ilgili özelliğin mutlaka girilmesini gerektirir. Kullanılabilecek olan bir diğer faydalı nitelikte (ki bana göre en işe yararlarından birisidir) RegularExpression'dır. İlk parametre olarak bir RegEx ifadesi almaktadır. Örneğimzde ModeratorMail özelliğinin taşıyacağı çalışma zamanı değerinin geçerli bir e-mail adresi olup olmadığı kontrol edilmektedir.

Entity'lere ait özellikler seviyesinde yapılabilen bu doğrulama işlemleri özellikle tipin görsel bileşenlere bağlanabildiği (Binding) uygulama çeşitlerinde epey kullanışlıdır. Bir başka deyişle Model View Controller (MVC) veya Model View View Model (MVVM) tarzı yapılarda değerlendirilebilir. Tabi Entity içerisinde çok fazla sayıda özellik olabilir ve her biri için ilgili doğrulama kriterleri ortaklık gösterebilir. Söz gelimi hiç bir Entity özelliğinin boş geçilmemesi istenebilir.

Diğer yandan nitelikler seviyesinde gerçekleştirilmesi pek kolay olmayan bazı doğrulama kuralları da söz konusu olabilir ve bunlar farklı yerlerde fonksiyonel hale gelmiş kütüphaneler içerisinde yer alabilirler. Bu durumda sanki Entity seviyesinde söz konusu olabilecek bir metod daha ideal olabilir. Bu tip bir vakayı karşılamak için Entity tipine yine System.ComponentModel.DataAnnotations içerisinde yer alan IValidatableObject arayüzünü uyarlamak ve beraberinde gelen Validate metodunu ezmek yeterlidir. Şimdi örneğimize aşağıdaki sınıf çizelgesinde görülen Player POCO (Plain Old CLR Object) tipini eklediğimizi düşünelim.

[![efv_2](/assets/images/2012/efv_2_thumb.png)](/assets/images/2012/efv_2.png)

```csharp
using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 
using System.Linq;

namespace HowTo_Validation 
{ 
    public class Player 
       :IValidatableObject 
    { 
        public int PlayerId { get; set; } 
        public string NickName { get; set; } 
        public string FirstName { get; set; } 
        public short Level { get; set; } 
        public string Country { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) 
        { 
            if (String.IsNullOrEmpty(NickName) || String.IsNullOrEmpty(FirstName) || String.IsNullOrEmpty(Country)) 
                yield return new ValidationResult("Nickname, FirstName veya Country değerleri boş bırakılamaz" 
                    , new string[] { "NickName", "FirstName", "Country" } 
                    );

            if (NickName.Length < 3 || NickName.Length>10) 
                yield return new ValidationResult("NickName en az 3 karakter en fazla 10 karakter olmalıdır",new string[]{"NickName"});

           if (!Constants.Levels.Contains(Level)) 
                yield return new ValidationResult("Geçersiz oyuncu seviyesi.", new string[] { "Level" });

            if (!Constants.Countries.Contains(Country)) 
                yield return new ValidationResult("Geçersiz ülke.", new string[] { "Country" });

        } 
    } 
}
```

Bu örnekte Player tipinin Nickname, FirstName, Country ve Level özellikleri için uygulanmış olan bazı doğrulama kriterleri olduğu görülmektedir. Dikkat edileceği üzere Validate metodu, doğrulama işlemine ait çalışma zamanı içeriğini ValidationContext tipinden olan parametre ile kontrol altına almaktadır. Bu özelliğin değeri elbetteki çalışma zamanında asıl Context nesnesi tarafından dolduralacak ve o Player entity tipine ait canlı nesne değerleri ile beslenecektir. Validate metodu geriye bir numaralandırıcı döndürmektedir. Dolayısıyla yield anahtar kelimesinden yararlanılabilir ve bu sayede n sayıda doğrulama ihlalinin asıl ortama döndürülmesi mümkün olabilir.

Dönüş koleksiyonu içerisinde yer alan tipler ValidationResult sınıfına ait örneklerdir. Bu sınıfa ait örnekler üretilirken genellikle ilk parametre olarak hata mesajı verilir. İkinci parametre ise ihlale sebebiyet veren özelliği ifade etmektedir. Bu iki bilgi yine çalışma zamanındaki Catch bloğunda yakalanan DbEntityValidationException örneği içerisinden alınabilir ve son kullanıcıya bilgilendirme de kullanılabilir.

Ancak bu yöntem için de bir dezavantaj da söz konusu olabilir. Uygulamada kullanılan Entity tiplerinin sayısı arttıkça ve benzer doğrulama kriterlerinin pek çok Entity için yapılması söz konusu ise attribute bazlı enjekte yöntemi terk edilebilir. Böyle bir durumda doğrudan Context nesnesi üzerinden bir doğrulama tekniği tercih edilebilir. Aynen aşağıda görüldüğü gibi.

Bu açılardan bakıldığında Entity Framework'deki doğrulama kontrolleri, ASP.Net tarafındaki hata yönetimini andırmaktadır. Asp.Net tarafında bildiğiniz üzere sırasıyla Metod, Sayfa (Page) ve uygulama (Application-global.asax.cs) seviyesinde hata kontrolleri gerçekleştirilir. EF'de de property'den başlayan, sınıf içi bir metod ile devam eden ve son olarak context nesnesi üzerinde ele alınabilen doğrulama enjekte noktaları mevcuttur. Hangi sırada çalıştıklarını merak ediyorsanız Debug edip denemenizi öneririm

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_123.png)

Belki de bu sıra duruma göre değişiklik arz, eder kim bilir

![Smile](/assets/images/2012/wlEmoticon-smile_55.png)

[![efv_3](/assets/images/2012/efv_3_thumb.png)](/assets/images/2012/efv_3.png)

```csharp
using System;

namespace HowTo_Validation 
{ 
    public class Author 
    { 
        public int AuthorId { get; set; } 
        public string FirstName { get; set; } 
        public string Scenario { get; set; } 
        public DateTime Birthday{ get; set; } 
    } 
}

using System; 
using System.Collections.Generic; 
using System.Data; 
using System.Data.Entity; 
using System.Data.Entity.Infrastructure; 
using System.Data.Entity.Validation;

namespace HowTo_Validation 
{ 
    public class GameContext 
        :DbContext 
    { 
        public DbSet<Player> Players { get; set; } 
        public DbSet<Layer> Layers { get; set; } 
        public DbSet<Author> Authors { get; set; }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, 
IDictionary<object, object> items) 
        { 
            var result = base.ValidateEntity(entityEntry, items);

            if (entityEntry.State == EntityState.Added && 
                entityEntry.Entity is Author) 
            { 
                var author = entityEntry.Entity as Author;

                if (author.Birthday > DateTime.Today) 
                { 
                   result.ValidationErrors.Add( 
                        new DbValidationError( 
                            "Yazar doğum tarihi", 
                            "Doğum tarihi bugünden büyük olamaz.") 
                            ); 
                } 
            } 
            return result; 
       } 
    } 
}
```

Standart olarak DbContext türevli olarak tasarlanan GameContext sınıfı içerisinde ValidateEntity metodunun ezildiği görülmektedir (override). Metod geriye DbEntityValidationResult tipnden bir örnek döndürmektedir. Olası n sayıdaki kural ihlali, bu tipin nesne örneğine ait ValidationErros koleksiyonunda toplanabilir.

Örnekte o anda üzerinde işlem yapılan Entity örneği DbEntityEntry tipinden olan entityEntry isimli metod parametresidir. Bu parametreden yararlanılarak State özelliğine bakılır ve ayrıca ilgili Entity'nin bir Author tipi olup olmadığı tespit edilir. Eğer yeni bir yazar ekleniyorsa bu durumda doğrulama işlemi yaptırılmaktadır. Sembolik olarak yazarın doğum tarihinin bu günün tarihinden büyük olmaması istenmiştir. Eğer bir ihlal söz konusu ise bu durumda ValidationErros özelliğinin işaret ettiği koleksiyona yeni bir DbValidationError örneği eklenir.

Buraya kadar ki örneklerimizle doğrulama kriterlerini 3 farklı seviyede ele alabildiğimizi gördük. Şimdi Program kodu içerisinde gerekli try...catch bloğunu uygulayarak örneğimizi test edelim ve çalışma zamanı sonuçlarını irdeleyelim.

```csharp
using System; 
using System.Collections.Generic; 
using System.Data.Entity; 
using System.Data.Entity.Validation;

namespace HowTo_Validation 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Code First için Doğrulama Teknikleri

            // Initialization stratejisi olarak modelde bir değişiklik olursa yeni baştan üretilmesini ifade ediyoruz 
            Database.SetInitializer<GameContext>(new DropCreateDatabaseIfModelChanges<GameContext>());

            // Context nesnesi örneklenir 
            using (GameContext context = new GameContext()) 
            { 
                try 
                { 
                    Player jedi = new Player(); 
                    jedi.NickName = "jd"; 
                    jedi.Level = 5000; 
                    jedi.Country = "Coroban";

                    context.Players.Add(jedi);

                    Layer layerSubZero = new Layer(); 
                    layerSubZero.Title = "bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla bla"; 
                    layerSubZero.ModeratorMail = "birmailiste"; 
                    layerSubZero.MaxPlayerCapacity = 200;

                    context.Layers.Add(layerSubZero);

                    Author me = new Author(); 
                    me.FirstName = "Burak"; 
                    me.Scenario = "Güzel bir oyun senaryosu var burada"; 
                    me.Birthday = new DateTime(2014, 1, 1);

                    context.Authors.Add(me); 
                                        
                    context.SaveChanges();

                } 
                catch (DbEntityValidationException exception) // Validasyon ile ilişkili Exception' lar yakalanır 
                { 
                   WriteErrosToConsole(exception.EntityValidationErrors); // Exception loglanmak amacıyla ilgili metoda gönderirilir 
                } 
            }

            #endregion 
        }

        private static int WriteErrosToConsole(IEnumerable<DbEntityValidationResult> validationErrors) 
        { 
            int errorCount = 0; 
            // Her bir Validation Error dolaşılmaya başlanır 
           foreach (DbEntityValidationResult validationError 
                in validationErrors) 
           { 
                // Doğrulama hatasına neden olan Entity bilgisi verilir 
                Console.WriteLine( 
                    "Entity bazlı hata : {0}", 
                    validationError.Entry.Entity);

                // Entity içerisinde doğrulama hatasına takılan özelliklerin her biri dolaşılır 
                foreach (DbValidationError propertyError 
                    in validationError.ValidationErrors) 
                { 
                    // Doğrulama hatasına takılan özelliğin adı ve hata mesajı yazdırılır 
                    Console.WriteLine( 
                        @" ""{0}"" özelliğinde : ""{1}"" hatası söz konusudur.", 
                        propertyError.PropertyName, 
                        propertyError.ErrorMessage); 
                } 
                errorCount++; 
            } 
            return errorCount; 
        } 
    } 
}
```

> Örnekte Code-First yaklaşımı kullanılmıştır. Bu yaklaşımda connection string bilgisi de önemlidir. Biz aksini belirtmedikçe uygulama SQL Express sürümü ve DbContext türevli tip adını baz alarak bir veritabanı oluşturacaktır. Biz örneğimizde aşağıdaki gibi bir connection string bilgisi kullandık.

Main metodu içerisinde üretilen GameContext örneği için örnek bir Player, Author ve Layer örneğinin eklenmesi söz konusudur. Bu örnekler eklendikten sonra yapılan SaveChanges çağrısı sırasında ilgili doğrulama kodları devreye girecek ve olası hatalar toplanarak catch bloğu içerisinde yakalanacaktır. Bu hataları kolay bir şekilde yazdırabilmek amacıyla, WriteErrorsToConsole isimli bir metoddan yararlanılmaktadır.

Dikkat edileceği üzere bu metod içerisinde ihlale neden olan Entity örneği DbEntityValidationResult örneklerine ait Entry.Entity özelliğinden yakalanmaktadır. Entity'den hemen bir alt seviye olan özelliklerdeki ihlallere inmek için de ValidationErros koleksiyonunda dolaşılmakta ve PropertyName ile ErrorMessage değerlerine bakılmaktadır. Örneği çalıştırdığımızda aşağıdaki ekran görüntüsündeki benzer bir hata mesajı ile karşılaşırız.

[![efv_4](/assets/images/2012/efv_4_thumb.png)](/assets/images/2012/efv_4.png)

Görüldüğü gibi tüm seviyelerdeki kural ihlalleri toplu olarak yakalanabilmiştir.

> Örneği çalıştırırken mutlaka debug edip adım adım ilerlemenizi öneririm. Bu sayede kodun sırasıyla hangi doğrulama kriterlerini çalıştırdığını daha net görebilirisiniz;)

Örnekte dikkati çeken noktalardan birisi de, ilgili doğrulama işlemlerinin SaveChanges metoduna yapılan çağrı ile devreye girmiş olmasıdır. Çok doğal olarak bu çağrıyı yapmadan önce bir yerlerde ilgili doğrulama kurallarını çalıştırmak ve olası ihlalleri toplamak isteyebiliriz. Bu tip bir durumda yine Context nesnesine ait olan GetValidationErros metodundan yararlanılabilir. Aşağıdaki kod parçasında olduğu gibi.

```csharp
var validationErros = context.GetValidationErrors(); 
var errorCount=WriteErrosToConsole(validationErros); 
if(errorCount==0) 
    context.SaveChanges();
```

GetValidationErros metodu IEnumerable tipinden bir referans döndürmektedir. Çok doğal olarak bu içerik en az 1 ihlal dahi içerse context nesne örneğine ait SaveChanges metodunun çağırılması istenmeyebilir.

Peki özellikle attribute seviyesinde yapılan doğrulama kontrollerini göz önüne alırsak, kendi özel kriterlerimizi içeren nitelikler tanımlayamaz mıyız? Elbetteki böyle bir esnekli var

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_123.png)

Nitekim System.ComponentModel.DataAnnotations isim alanı altında yer alan doğrulama kriterlerinin ortak özelliği, ValidationAttribute niteliğinden türemiş olmalarıdır.

[![efv_5](/assets/images/2012/efv_5_thumb.png)](/assets/images/2012/efv_5.png)

ValidationAttribute niteliği de doğal olarak Attribute tipinden türemektedir. Öyleyse kendi doğrulama niteliklerimizi yazmanın bir yolunu bulduğumuzu ifade edebiliriz

![Winking smile](/assets/images/2012/wlEmoticon-winkingsmile_123.png)

Söz gelimi Author tipimize SocialSecurityNumber isimli string bir özellik eklediğimizi ve buraya girilen değerlerin geçerli bir numara olup olmadığını denetleyecek bir doğrulama niteliği geliştirmek istediğimizi farz edelim. Aşağıdaki şekilde ilerleyebiliriz.

[![efv_6](/assets/images/2012/efv_6_thumb.png)](/assets/images/2012/efv_6.png)

```csharp
using System; 
using System.ComponentModel.DataAnnotations;

namespace HowTo_Validation 
{ 
    [AttributeUsage(AttributeTargets.Property)] 
    public class SSNValidatorAttribute 
        :ValidationAttribute 
    { 
        private bool CheckIsValid(string ssn) 
        { 
            //TODO@Burak Do Something for SSN check

            return false; 
        }

        public override bool IsValid(object value) 
        { 
            return CheckIsValid(value.ToString()); 
        } 
    } 
}
```

Tabi duruma göre söz konusu niteliğin ezmesi gereken üye sayısı daha fazla olabilir. Biz örneğimizde sadece IsValid metodunu ezdik. Çalışma zamanında bu metoda girildiğinde object tipinden olan value parametresinin değeri, niteliğin uygulandığı özelliğin çalışma zamanındaki içeriği olacaktır. (Burada sembolik olarak kontrol işlemini üstlenen ayrı bir metod private olarak tanımlanmıştır. Gerçek hayatta bu metod gerçekten de harici bir servisi çağırarak denetleme işlemini yapabilir)

[![efv_7](/assets/images/2012/efv_7_thumb.png)](/assets/images/2012/efv_7.png)

Bunu kontrol ederek duruma göre geriye true veya false değer döndürmemiz yeterlidir. Niteliği Author Entity tipi için aşağıdaki kod parçasında görüldüğü şekilde uygulayabiliriz.

```csharp
using System;

namespace HowTo_Validation 
{ 
    public class Author 
    { 
        public int AuthorId { get; set; } 
        public string FirstName { get; set; } 
        public string Scenario { get; set; } 
        public DateTime Birthday{ get; set; } 
        
        [SSNValidator(ErrorMessage="Hatalı sosyal güvenlik numarası")] 
        public string SocialSecurityNumber { get; set; } 
    } 
}
```

Şu andaki test kodumuz her vaziyette SSN doğrulamasında false değer üretecektir. Sonuçta ekran çıktısını bu işlem de aşağıdakine benzer bir şekilde yansıtılacaktır.

[![efv_8](/assets/images/2012/efv_8_thumb.png)](/assets/images/2012/efv_8.png)

Özetle Entity Framework tarafındaki doğrulama işlemlerini Entity seviyesinde nitelikler (Attribute) ve IValidatableObject arayüzü sayesinde gerçekleştirebilirken, Context tipi seviyesinde de ezilebilen (overridable) ValidateEntity metodu içerisinde yapabiliriz. Bu makalemizde çok basit seviyede de olsa, Code-First Entity Framework tabanlı doğrulama işlemlerini ele almaya çalıştık. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[HowTo_Validation.zip (2,59 mb)](/assets/files/2012/HowTo_Validation.zip)