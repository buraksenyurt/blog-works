---
title: "Workflow Foundation 4.0 - Kodlama Zamanında Doğrulama(Validation)"
pubDate: 2010-01-15 01:08:00
categories:
  - WF 4.0 Beta 2
tags:
  - WF 4.0 Beta 2
  - C#
  - Workflow Foundation
  - Authentication
  - Visual Studio
---

# Workflow Foundation 4.0 - Kodlama Zamanında Doğrulama(Validation)
![blg110_Giris.jpg](images/blg110_Giris.jpg)

Merhaba Arkadaşlar,

Bazen nerede duracağımızı bilmemiz gerekir ve bazende, mümkün olduğunca erken durup bazı şeyleri değiştirerek ilerlememiz...Bu teori yazılım geliştirmeninde pek çok noktasında karşımıza çıkmaktadır. Durmamız gereken noktalardan birisi, uygulamaların ürettiği ve önceden fark edebileceğimiz hatalardır (Genellikle Exception'ları düşünebiliriz). Ancak bazı olası hataların uygulamaların çalışması sırasında değil, çalıştırılmaya başlamadan önce bilinmesinde hem zaman hemde maliyet kazancı açısından yarar vardır. Şimdi elimizdeki materyalleri bir düşünelim.

Ürün geliştirmek için kullandığımız Visual Studio gibi gelişmiş bir araç,.Net Framework platformu vb...O halde bazı hataların çalışma zamanı yerine daha geliştirme aşamasındayken IDE üzerinde fark edilmesinin önemli olduğunu söyleyebiliriz. Peki geliştirme safhasında, örneğin bir Workflow aktivitesini kullanırken...Hımmmm...Sanırım nereye varmak istediğimi anladınız.

![Wink](images/smiley-wink.gif)

Bu yazımızda, özel aktivite bileşenlerinin kodlama zamanındayken olası hataları nasıl bildirebileceğini ve böylece doğrulamanın (Validation) nasıl sağlanabileceğini incelemeye çalışacağız.

Bundan önceki yazılarımızda özel aktivite bileşenlerimizi nasıl geliştirebileceğimizi incelemeye çalışmıştık. Bu amaçla yazdığımız örneklermizde CodeActivity ve AsyncCodeActivity türevli bileşenler geliştirmiştik. Özel aktivite bileşenleri geliştirilmesi sırasında dikkat edilmesi gereken önemli konulardan biriside kodlama zamanında gerçekleştirilmesini istediğimiz doğrulama (Validation) işlemleridir. Doğrulama için Workflow Foundation 4.0 tarafında kullanılabilen birden fazla teknik bulunmaktadır. Nitelik (Attribute) bazlı kullanım dışında Imperative ve Declerative olaraktan da doğrulama işlemlerini gerçekleştirebiliriz. Aslında konuyu anlamanın en güzel yolu öncelikli olarak sorunu ortaya koymaktan geçmektedir. Bu sebepten aşağıdaki gibi bir CodeActivity bileşeni tasarladığımızı düşünelim.

```csharp
using System.Activities;
using System.IO;
using System;
using System.Activities.Expressions;

namespace CustomActivities
{
    public sealed class FileCopy 
        : CodeActivity
    {
        public InArgument<string> Source { get; set; }
        public InArgument<string> Destination { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            File.Copy(Source.Get(context), Destination.Get(context));
        }
    }
}
```

FileCopy isimli aktivite bileşenimizin görevi Source özelliğinde belirtilen dosyayı, Destination özelliğinde belirtilen yere kopyalamaktır. Bu işlem için CodeActivity tarafından gelip ezilen (override) Execute metodu içerisinde, File sınıfının static Copy metodundan yararlanılmaktadır. Ne varki bu aktivite bileşeninin özellikle çalışma zamanında üreteceği bazı sorunlar vardır. Aslında bunları şimdiden tahmin etmemiz son derece kolaydır.

Herşeyden önce, InArgument tipinden olan Source ve Destination özelliklerinin boş geçilmemesi yani veri ile doldurulması şarttır. Nitekim null verilerin Copy metodu içerisinde kullanılması söz konusu olamaz. Diğer yandan Source özelliğinde belirtilen dosyanın, sistemde gerçekten var olması gerekmektedir. Dolayısıyla Source özelliğinde bir değer olsa bile bunun geçerli bir dosya olup olmadığına bakılmalıdır. Üçüncü olaraktan Destination özelliğinde belirtilen dosya adının geçerli olması ve belkide Source ile belirtilen dosya uzantısında olması gerekmektedir. Hatta hedef dosya zaten var ise üzerine yazılması durumu söz konusudur. Bu vakaların herhangibirinin çalışma zamanında gerçeklenmesi sonrası istisnalar (Exceptions) ile karşılaşılması kaçınılmazdır. Söz gelimi aktivitemizi bu haliyle örnek bir Workflow içerisinde icra ettirdiğimizde çalışma zamanında aşağıdaki istisna mesajını alırız.

![blg110_Exception.gif](images/blg110_Exception.gif)

Source özelliğinde null değer olduğundan Copy operasyonunun icrası sırasında ArgumentNullException alınmıştır. Oysaki bu hatanın çalışma zamanında değil kodlama zamanında, yani Visual Studio ortamında daha tasarımı gerçekleştirirken farketmemiz çok önemlidir. Bu bize zaman ve maliyet kazancı olarak geri dönecektir. İşte doğrulamanın sağlanması gereken yerlerden birisi burasıdır. Peki bunu nasıl gerçekleştirebiliriz? Öncelikli olarak Source ve Destination isimli InArgument tipinden olan özelliklerin boş bırakılmamasını sağlamalıyız. Bunun için ilgili özellikleri RequiredArgument niteliği ile aşağıdaki kod parçasında görüldüğü gibi işaretlememiz yeterli olacaktır.

```csharp
[RequiredArgument]
public InArgument<string> Source { get; set; }
[RequiredArgument]
public InArgument<string> Destination { get; set; }
```

Bu durumda tasarım zamanında aşağıdaki görüntü ile karşılaşırız.

![blg110_DesignTime.gif](images/blg110_DesignTime.gif)

Dikkat edileceği üzere Source ve Destination değelerinin doldurulması zorunlu hale getirilmiştir. Üstelik bu durum derleme işleminden sonra açık bir şekilde adeta geliştiricinin gözüne sokulmaktadır.

![Laughing](images/smiley-laughing.gif)

RequiredArgument niteliği ile ilişkili olarak dikkat edilmesi gereken noktalardan biriside Argument tiplerine uygulandığında işe yarıyor olmasıdır. Yani Soruce ve Destination özelliklerinin string tipinden olmaları gibi bir durumda bu niteliğin bir etkisi olmayacaktır.

İlk vakayı çözümledik. Artık dosya adlarını girdiğimizi düşünebiliriz. Sıradaki sorun Source özelliğine girilen dosyanın sistemde olmaması halinde ortaya çıkacaktır. Buna göre yine kopyalama işlemi sırasında bir çalışma zamanı hatası alınacaktır. Durumu irdeleyebilmek için Source ve Destination özelliklerini aşağıdaki şekilde görüldüğü gibi ayarladığımızı düşünelim.

![blg110_SourceOk.gif](images/blg110_SourceOk.gif)

Bu durumda çalışma zamanında aşağıdaki hata mesajını alırız.

![blg110_Exception2.gif](images/blg110_Exception2.gif)

Tabi burada c:\ klasörü içerisinde Source.txt isimli bir dosyanın gerçektende var olmadığını düşünüyoruz. Buradaki istisnaya göre kaynak dosyanın önceden kontrol edilmesi ve derleme işleminden sonra geliştiriciye bir uyarı veya hata mesajı ile durumun bildirilmesi istenebilir. Ancak burada basit bir nitelik yardımıyla aşabileceğimizin ötesinde bir durum vardır. Nitekim doğrulama için özel bir iş mantığı (Bussines Logic) bulunmaktadır. İşte bu tip doğrulamaları gerçekleştirebilmek için CodeActivity ve NativeActivity türevlerinde CacheMetadata isimli metodun ezilmesi ve Visual Studio ortamına doğrulama ile ilişkili bilgilendirmenin yapılması gerekmektedir. Sözün özü FileCopy aktivite bileşenimizi aşağıdaki hale getirmemiz yeterli olacaktır

![Wink](images/smiley-wink.gif)

```csharp
using System.Activities;
using System.IO;
using System;
using System.Activities.Expressions;

namespace CustomActivities
{
    public sealed class FileCopy 
        : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Source { get; set; }
        [RequiredArgument]
        public InArgument<string> Destination { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            File.Copy(Source.Get(context), Destination.Get(context));
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
            string source = ((Literal<string>)Source.Expression).Value;

            if (!File.Exists(source))
                metadata.AddValidationError(new System.Activities.Validation.ValidationError("Kaynak dosya bulunamadı.", true));
        }
    }
}
```

Burada Source özelliğinin değerine bakılmakta ve belirtilen dosyanın sistemde gerçekten var olup olmadığı tespit edilmektedir. Eğer söz konusu dosya sistemde yok ise "Kaynak dosya bulunamadı." içeriğine sahip bir uyarı mesajı (Warning) üretilir. Uyarı mesajı diyoruz çünkü sonda yer alan true değeri bunu sağlamaktadır. Yine derleme işleminden sonra oluşacak durum aşağıdaki gibidir.

![blg110_Warning.gif](images/blg110_Warning.gif)

Tabi eğer true yerine false değerini kullanırsak aşağıda görülen nur topu gibi error'un sahibi oluruz.

![Smile](images/smiley-smile.gif)

![blg110_WarningFalse.gif](images/blg110_WarningFalse.gif)

Bu adımdan sonra geçerli bir kaynak dosyayı Source özelliğine atayarak devam edersek Destination özelliğine atanan değer ile ilişkili kontrolleri yapmamız gerektiği sonucuna varabiliriz. Buna göre hedef dosyanın kaynak dosya ile uzantı bakımından uyumlu olması sağlanabilir. Tabiki arka arkaya yapılan çalıştırmalar sonrasında hedef dosya zaten var ise overwrite ile ilişkili hataların oluşması da söz konusudur. Bu iki doğrulama işlemini siz değerli okurlarıma bir antrenman olması için bırakıyorum

![Wink](images/smiley-wink.gif)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[CustomActivityValidation.rar (50,30 kb)](assets/CustomActivityValidation.rar)
