---
layout: post
title: "Workflow Foundation 4.0 - Declarative Validation [Beta 2]"
date: 2010-01-19 06:00:00 +0300
categories:
  - wf-4-0-beta-2
tags:
  - workflow-foundation
---
Sakin bir Cuma gününde bilgisayarın başında kahvemi yudumlarken ve M&M drajelerinden avuç avuç yerken araştırmalarıma devam ediyordum. Bir süredir Workflow Foundation 4.0 ile birlikte gelen yenilikleri incelediğimden takip ettiğim bloglar ve MSDN üzerinde bu konu ile ilişkili yazıları okumaktaydım. Özelliklede son iki yazımda üzerinde durmaya çalıştığım özel aktivite bileşenlerinin doğrulanması konusunu irdelemekteydim. Bu yazımızda doğrulama (Validation) ile ilişkili araştırmalarımı sizlerle paylaşmaya devam ediyor olacağım.

![blg111_Giris.gif](/assets/images/2010/blg111_Giris.gif)

Doğrulama işlemlerinin çeşitlerine baktığımızda Declarative Constraint isimli bir yaklaşımın daha olduğu görülmektedir. Bu yaklaşıma göre bir aktivite ile ilişkili doğrulama mantığının kısıt olaraktan (Constraint) ayrı bir tip ve metod içerisinde konuşlandırılması mümkündür. (Hatta kod dışında XAML bazlı olaraktan kısıtların konulmasıda söz konusudur) Workflow Foundation alt yapısında bu tip dekleratif doğrulamalar için Constraint sınıfından yararlanılmaktadır. Constraint tipi aslında NativeActivity türevidir. Bir başka deyişle bir aktivitedir.

![blg111_Constraint.gif](/assets/images/2010/blg111_Constraint.gif)

Yukarıdaki şekildende görülebileceği gibi Constraint abstract bir sınıftır (dolayısıyla kendisinden türeyen tiplerin mutlaka uyması gereken kuralları bildiren, örneklenemeyen ama kendisinden türeyen tip örneklerini taşıyabilen bir sınıftır) ve NaticeActivity tipinden türemektedir. Bu türetme nedeniyle aslında Workflow çalışma zamanının çeşitli materyallerine erişebildiğini (Scheduling, Bookmarks vb...) söyleyebiliriz. Peki pratikte kendi geliştireceğimiz aktivite bileşenleri için gerekli kısıtları nasıl koyabiliriz? MSDN üzerinde bu konu ile ilişkili olarak geliştirilen basit örnekte bir aktivite bileşeninin DisplayName özelliğinin 2 karakterden fazla olması gerekliliğinin örneklendiği görülmektedir. Bizde buna benzer bir kısıtlama geliştiriyor olacağız. Ancak örneğin farklı olması açısından, hayali yazılım şirketinin kod standartlarına göre DisplayName özelliğinin Chinook ön eki ile başlaması için bir kısıt getireceğiz. İşte Activity Library içerisinde tuttuğumuz örnek sınıf kodlarımız.

```csharp
using System.Activities;
using System.Activities.Validation;

namespace CustomActivities
{
    public static class ActivityConstraints
    {
        // Constraint oluşturup geriye döndürecek basit bir static metod
        public static Constraint ValidateActivityDisplayNameForCompanyCodeStandards()
        {
            DelegateInArgument<Activity> element = new DelegateInArgument<Activity>();

            // Herhangibir Activity(T generic tipi olarak Activity kullanıldığından) bileşeninin doğrulanmasında kullanılacak olan Constraint tipi örneklenir. 
            // Constraint tipi aslında NativeActivity türevli bir Activity bileşenidir.
            Constraint<Activity> constraint = new Constraint<Activity>
            {
                // Body kısmı doğrulama mantığını içermektedir ve ActivityAction tipindendir                
                Body = new ActivityAction<Activity, ValidationContext>
                {
                    Argument1 = element,                    
                    Handler = new AssertValidation
                    {
                        // Warning bilgisi gösterilmeyecektir. Yani Error mesajı verilecektir. IsWarning özelliğinin varsayılan değeri false' dur.
                        IsWarning=false,
                        // e, ActivityContext tipinden bir referanstır. Dolayısıyla Constraint' in uygulanacağı aktivite bileşeninin güncel içeriğine erişilebilmesi mümkündür.
                        // Örnek doğrulamaya göre Actitiy örneğinin DisplayName özelliğinin Chinnok kelimesi ile başlaması beklenmektedir.
                        Assertion=new InArgument<bool>(
                            e=>                                
                                element.Get(e).DisplayName.StartsWith("Chinook")
                            ),
                            // Eğer Chinook ismi ile başlanılmıyorsa bir hata mesajı verilir.
                        Message=new InArgument<string>("Şirketin kod standartları gereği, özel Activity adlarının Chinook ile başlaması gerekmektedir."),
                        DisplayName="DisplayNameValidationActivity"                         
                    }
                }
            };
            return constraint;
        }
    }
}
```

ActivityConstraints isimli static sınıf içerisinde yer alan ValidateActivityDisplayNameForCompanyCodeStandards isimli metod geriye Constraint tipinden bir referans döndürmektedir. Constraint tipinin üretimi sırasında dikkat edileceği üzere Handler özelliğine AssertValidation tipinden bir referans atanmaktadır. İşte bu sınıfın örneklenmesi sırasında kullanılan Assertion özelliği ilede bir Expression tanımlaması yapılmakta ve bu kısıtın uygulandığı Activity bileşeninin DisplayName özelliğinin Chinook ile başlayıp başlamadığı kontrol edilmektedir. Bu ifadeden dönecek değer göre Message özelliğine atanan bilginin derleme zamanında gösterilmesi sağlanmaktadir. IsWarning özelliği varsayılan olarak false değere sahiptir ve buna göre mesajın bir Error olarak gösterilmesi sağlanmaktadır. Ancak bu özelliğe true değerini atayaraktan Warning olarak gösterilmesi de sağlanabilir. Peki bu kısıt bir aktivite bileşenine nasıl uygulanabilir? Aslında bunun için geliştiriken aktiviteye bir bildirimde bulunulması yeterlidir. Aşağıdaki kod parçasında yer alan aktivite bileşeninde bu durum ele alınmaktadır.

```csharp
using System.Activities;
using System.Activities.Validation;

namespace CustomActivities
{
    public enum LogSource
    {
        File,
        Database,
        WebService,
        System
    }

    // Loglama yapan örnek bir aktivitedir.
    public sealed class LogActivity 
        : CodeActivity<bool>
    {
        public InArgument<LogSource> LogSourceType { get; set; }

        public LogActivity()
        {
            // Constraint' lerin bir aktivite ile ilişkilendirilebilmesi için base referans üzerinden ilgili koleksiyona eklenmesi gerekmektedir. 
            // Constraints özelliği bir koleksiyonu referans ettiği için bir aktiviteye birden fazla Constraint yüklenmesi mümkündür.
            base.Constraints.Add(ActivityConstraints.ValidateActivityDisplayNameForCompanyCodeStandards());
        }

        protected override bool Execute(CodeActivityContext context)
        {
            //TODO@Burak: Gerçektende loglama işlemi yapılması için gerekli kodlar yazılmalı
            switch (LogSourceType.Get(context))
            {
                case LogSource.File:
                    break;
                case LogSource.Database:
                    break;
                case LogSource.WebService:
                    break;
                case LogSource.System:
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}
```

Burada odaklanılması gereken tek bir yer vardır...Yapıcı metodun (Constructor) içerisinde yer alan kod satırı. Dikkat edileceği üzere Constraints özelliği üzerinden ValidateActivityDisplayNameForCompanyCodeStandards metodunun dönüş referansının ilgili koleksiyona eklenmesi sağlanmaktadır. Buna göre, LogActivity bileşeninin Visual Studio ortamında kullanıldığı durumlarda DisplayName özelliğinin ilgili kısıta göre kontrol edileceği bildirilmiş olmaktadır. Dilerseniz birde bileşeni test edelim. İşte bileşeni örnek bir Workflow üzerine ilk kez sürükleyip bıraktığımızdaki durum;

![blg111_Error.gif](/assets/images/2010/blg111_Error.gif)

Görüldüğü üzere DisplayName özelliği için bir hata mesajı alınmıştır. Buna göre Chinook ön ekin kullandığımızda sorunun ortadan kalktığı görülebilir.

![blg111_NoError.gif](/assets/images/2010/blg111_NoError.gif)

Constraint kullanımı dikkat edileceği üzere herhangibir aktivite bileşeni için kısıt koyabilmeyi olanaklı hale getirmektedir. Constraint kullanımında farklı durumlarda söz konusudur. Örneğin bir aktivitenin içerisinde yer alan tüm alt aktiviteler için geçerli olacak kısıtların konulmasıda mümkün olabilir. Bu son derece doğaldır nitekim Constraint sınıfı örneklenirken, ilişkilendirildiği aktivite içeriğine erişebilmektedir. Burada DelegateInArgument temsilci tipinin büyük rolü vardır. Öyleki Expression tanımlamasının yapıldığı ve doğrulama mantığının gerçekleştirildiği yerde, güncel aktivite referansına ulaşmak için element.Get (e) söz dizimi kullanılmaktadır. Tabiki bu yazımızda Declarative Constraint kullanımını çok basit seviyede ele almaya çalıştık. En güncel ve detaylı bilgiyi elbetteki [MSDN](http://msdn.microsoft.com/en-us/library/ee358736(VS.100).aspx) üzerinde bulabilirsiniz. Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[DeclerativeConstraints.rar (50,29 kb)](/assets/files/2010/DeclerativeConstraints.rar)
