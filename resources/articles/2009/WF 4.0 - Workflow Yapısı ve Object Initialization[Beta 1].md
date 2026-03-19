---
title: "WF 4.0 - Workflow Yapısı ve Object Initialization[Beta 1]"
pubDate: 2009-10-04 23:29:00
categories:
  - WF 4.0 Beta 1
tags:
  - WF 4.0 Beta 1
  - C#
  - Workflow Foundation
  - Visual Studio
---

# WF 4.0 - Workflow Yapısı ve Object Initialization[Beta 1]
Merhaba Arkadaşlar,

Workflow Foundation 4.0 ile ilgili yenilikleri araştırdığım şu günlerde, yaptığım araştırmalar sırasında ilgimi çeken noktalardan biriside, bir Workflow'un kod tarafında tek bir ifade satırı ile oluşturulabiliyor olmasıydı. Burada Workflow sisteminin hiyerarşik yapısının, Object Oriented seviyede etkili bir kullanımının söz konusu olduğunu belirtmek isterim. Ancak konuya çekirdek bilgilerden başlayarak yaklaşmakta yarar var.

Workflow içerisindeki lego parçalarının temelini aktiviteler (Activities) oluşturmaktadır. WF 4.0 mimarsinde tüm aktiviteler WorkflowElement bileşeninden türemektedir. Aktivitleri aslında Workflow'ların iş birimleri (Work Units) olarak düşünebiliriz. İki ve daha fazla iş biriminin bir araya gelerekten aktivite oluşturmaları da çok doğal olarak mümkündür. Aslında buradan ilginç olan bir tespit vardır. Bir aktivite, hiyerarşinin en üstünde yer alıyorsa bir Workflow halini alır ve kendi içerisinde pek çok aktiviteyi barındırabilir. Bunu şu şekilde de düşünebiliriz; "Bir metodun kendi içerisinde birden fazla metodu çağırması".

Nitekim metodun kendi içerisinde çağırdığı ardışıl fonksiyonlar, yukarıdan aşağıya doğru hareket eden bir iş akışını oluşturmaktadır. Bu açıdan bakıldığında Workflow Foundation olmasa dahi, kod bazında iş akışlarının en temel seviyede fonksiyonlar yardımıyla gerçekleştirilebileceğini bilmek gerekir (Hatta çalıştığım son projede bu tip bir mimari kullanılmaktadır). Workflow Foundation 3.X sürümünden beridir Sequence gibi üst seviye (Top Level) aktiviteleri barındırmaktadır. Bunlar Top Level olarak kullanıldıklarında bir Workflow'u ifade etmektedir. Tabiki Sequence örnekleri kendi içlerinde başka Sequence örnekleri de barındıralabilirler.

Burada ilginç olan noktalardan birisi, Workflow'ları sadece tek bir ifade içerisinde oluşturabilmemizdir.

![Surprised](images/smiley-surprised.gif)

Bunun için nesne başlatıcılarından (Object Initializers) yararlanılmaktadır. Nasıl mı? Gelin Visual Studio 2010 Beta 1 üzerinde basit bir Console Application oluşturup Program.cs içeriğini aşağıdaki gibi oluşturduğumuz düşünelim.

> Not: Console uygulaması üzerinde Workflow aktivitileri kullanılacağından, projeye System.Activities.dll Assembly'ının referans edilmesi gerekmektedir.

```csharp
namespace WorkflowStructure
{
    using System;
    using System.Activities;
    using System.Activities.Statements;

    class Program
    {
        static void Main(string[] args)
        {
            // Yeni bir Sequence aktivitesi oluşturulur. Top Level olduğu için workflow' un kendisidir.
            // Tek satırlık ifade içerisinde bir Workflow tanımlandığına dikkat edilmelidir
            Sequence flow1 =new Sequence
            {
                DisplayName = "Hello Workflow World",
                // Activity tipinden elamanlar taşıyan koleksiyonun içerisinde alt aktiviteler tanımlanır 
                Activities =
                {
                    new WriteLine{ DisplayName="Workflow Start", Text="Starting..."},//Basit bir WriteLine aktivitesi
                    new InvokeMethod{ DisplayName="5 Times Say Hello", MethodName="SayHello", TargetType=typeof(Logic)} // Logic sınıfından SayHello metodunu çağıracak olan InvokeMethod aktivitesi tanımlanır 
                }                 
            };

            WorkflowInvoker.Invoke(flow1); // Workflow örneği çalıştırılır :)
        }
    }

    // Harici bir sınıf ve metod
    class Logic
    {
        public static void SayHello()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("\tHello");
            }
        }
    }
}
```

Bu örnek kod parçasında Sequence tipinden bir nesne örneği oluşturulmaktadır. Nesne oluşturulurken, Activities özelliği içerisinde alt aktivite örnekleri kullanılmıştır. Activities özelliği Collection tipinden bir koleksiyonu işaret etmektedir. Dolayısıyla herhangibir Activity referansına ait örnek, söz konusu koleksiyona eklenerek akışın gövdersi oluşturulabilir. Örnekte ilk olarak System.Activities.Statements isim alanında yer alan WriteLine aktivite bileşeni kullanılarak ekrana basit bir çıktı verilmektedir. Takip eden adımda, InvokeMethod aktivite bileşeni kullanılmaktadır. Bu bileşenin özellikleri ile, Logic sınıfı içerisinde yer alan SayHello metodunun çalıştırılacağı belirtilmiştir (Burada TargetType veya TargetObject özelliklerinden birisinin mutlaka set edilmesi gerekir.

Böylece MethodName özelliğinde belirtilen fonskiyonun çalışma zamanında hangi tipe ait nese örneği içerisinden çağırılacağı belirtilmiş olur) Tabiki flow1 isimli Sequence nesne örneğinin çalıştırılması için WorkflowInvoker tipi üzerinden static Invoke metodu kullanılmıştır. Olaya noktalı virgüller açısından baktığımızda sadece iki satırda bir Workflow'un tasarlanıp yürütüldüğünü ifade edebiliriz. Ancak görsel bir tasarım ortamının olması elbetteki çok daha mühimdir ve tercih edilmelidir. Nitekim gerçek hayat çözümlerindeki iş akışlarının çoğu bu kadar basit Workflow örnekleri ile ifade edilememektedir. Geliştirdiğimiz Workflow herhangibir anlam içermesede

![Smile](images/smiley-smile.gif)

önemli olan, tek satırlık bir ifade ile object initializer kavramından da yararlanarak, bir Workflow örneğinin tesis edilebileceğinin farkında olmaktır. Örneği yürüttüğümüzde çalışma zamanı için aşağıdaki sonuçları aldığımızı görürüz.

![blg88_Runtime.gif](images/blg88_Runtime.gif)

Görüldüğü üzere akış başarılı bir şekilde işletilmiştir. Bu kısa yazımızda Workflow yapısının kod tarafında Object Initializers yardımıyla ele alınışını incelemeye çalıştık. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[WorkflowStructure.rar (19,38 kb)](assets/WorkflowStructure.rar)
