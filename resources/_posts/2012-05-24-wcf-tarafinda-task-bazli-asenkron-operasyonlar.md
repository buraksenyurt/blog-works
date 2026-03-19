---
layout: post
title: "WCF Tarafında Task Bazlı Asenkron Operasyonlar"
date: 2012-05-24 14:50:00 +0300
description: "Uzun bir zamandır .Net Framework içerisinde, fonksiyonların asenkronize edilmesi üzerinde çalışılmaktadır. Daha önceleri Thread bazlı veya Delegate tipleri ile gerçekleştirdiğimiz asenkron çağırımlar, .Net Framework 4.0’ a gömülü olarak gelen Task Parallel Library sayesinde daha da gelişmiş ve alt yapının her noktasına enjekte edilebilir olmuştur. Şu günlerde .Net Framework 4.5 ile birlikte gündeme gelen ve uzun zamandır da haberdar olduğumuz async, await gibi anahtar kelimeler de, temel de Task tiplerine dayanmaktadır. Bir başka deyişle TPL kütüphanesi ve içeriği, ilerleyen zamanlarda .Net Framework’ ün pek çok önemli alt yapısında etkisini hissettirecektir."
categories:
  - wcf-4-0
tags:
  - wcf-4-0
  - csharp
  - dotnet
  - ado-net
  - wcf
  - async-await
  - task-parallel-library
  - threading
  - concurrency
  - delegates
  - generics
---
# WCF Tarafında Task Bazlı Asenkron Operasyonlar
[![delegating](/assets/images/2012/delegating_thumb.jpg)](/assets/images/2012/delegating.jpg) Merhaba Arkadaşlar,

Yandaki karikatür, aşağıdaki yazıyı bitirdiğim zaman aradığım giriş resmi ile ilişkili olarak karşıma çıkan örneklerden sadece bir tanesiydi. Beni epey bir güldürdüğünü ve neşelendirdiğini ifade edebilirim

![Laughing](/assets/images/2012/smiley-laughing.gif)

Konumuz işlerimizi birilerine yönlendirip yan gelip yatmak değil elbette, ama ona benzer olduğunu ifade edebilirim.

Bu adamın görevlerini başkaların atadıktan sonra, söz konusu işler yapılırken başka işlere (örneğin koltuğunda şöyle bir geriye doğru yaslanarak vakit geçirmek) yönelebildiğine odaklanmaya çalışalım..Net Framework tarafındaki Delegate tipleri de benzer bir ihtiyacı karşılamıyor mu?

![Wink](/assets/images/2012/smiley-wink.gif)

Asenkron olarak fonksiyonların çağırılabilmesini sağlamak (Elbette başka yetenekleri de var ama bu en önemlileri arasında sayılabilir)

Uzun bir zamandır.Net Framework içerisinde, fonksiyonların asenkronize edilmesi üzerinde çalışılmaktadır. Daha önceleri Thread bazlı veya Delegate tipleri ile gerçekleştirdiğimiz asenkron çağırımlar,.Net Framework 4.0’ a gömülü olarak gelen Task Parallel Library sayesinde daha da gelişmiş ve alt yapının her noktasına enjekte edilebilir olmuştur. Şu günlerde.Net Framework 4.5 ile birlikte gündeme gelen ve uzun zamandır da haberdar olduğumuz async, await gibi anahtar kelimeler de, temel de Task tiplerine dayanmaktadır. Bir başka deyişle TPL kütüphanesi ve içeriği, ilerleyen zamanlarda.Net Framework’ ün pek çok önemli alt yapısında etkisini hissettirecektir.

Bu yazımızda Task tiplerinden yararlanarak, WCF (Windows Communication Foundation) servislerinde asenkron operasyon tanımlamalarının nasıl yapılabileceğini ve incelemeye çalışıyor olacağız. Gerçekleştirmeyi planladığımız işlemlerde önemli olan nokta ise, asenkron yürütmelerin istemci (Client) tarafında değil, servis tarafındaki operasyonlar için söz konusu olmasıdır. Bir başka deyişle eş zamanlı olarak çalışabilen (Concurrent) servis operasyonlarının, Task tiplerinden yararlanarak nasıl asenkron hale getirilebileceğini görmeye çalışacağız. Elbette bu asenkronize edilmiş metodlar servis tarafında değerlendirilen bir yaklaşımı içerecektir.

Aslında bir servis operasyonunun asenkron çalışacak hale getirilmesi, delegate tipleri ile kullanabildiğimiz (hatta Ado.Net 2.0’ dan bu yana pek çok XCommand gibi tipinde var olan) Begin… ve End… ön ekli metod oluşumlarının uygulanmasından ibarettir. Aşağıdaki örnek kod parçasında sadece hatırlatıcı olması açısından bir Delegate tipi üzerinden ilgili BeginInvoke ve EndInvoke metodlarına nasıl ulaşılabildiği gösterilmeye çalışılmıştır. Örnekte Async Callback modeli değerlendirilmiştir. Bir başla deyişle asenkron olarak başlatılan metod işleyişini tamamladığında, uygulama ortamındaki başka bir geri bildirim fonksiyonu tetiklenmektedir.

```csharp
using System; 
using System.Collections.Generic;

namespace AzonTestClient 
{ 
    // Örnek bir generic temsilci 
    delegate int SaveToFileDelegate<T>(IEnumerable<T> list);

    class Program 
    { 
        static void Main(string[] args) 
        { 
            #region Async Callback modeli

            // Temsilci örneklenir 
            SaveToFileDelegate<object> dlg=new SaveToFileDelegate<object>(SaveToFile); 
            // BeginInvoke çağrısı ile dlg' nin işaret ettiği SaveToFile metodunun çağırılması ve kodun ifade sonundan itibaren akmaya devam etmesi sağlanır 
            // ilk parametre SaveToFile metodunun alacağı değişken, ikinci parametre işlem bittiğinde tetiklenecek geri bildirim fonksiyonunun işaretçisi olan temsilci, üçüncü parametre ise geri bildirim metodunda AsyncState özelliği üzerinden yakalanacan Delegate referansı 
            IAsyncResult asyncResult = dlg.BeginInvoke(new List<object>(), new AsyncCallback(Callback), dlg); 
            // akış buradan kesilmeden devam eder

            #endregion

            Console.ReadLine(); 
        }

        // Uzun süreli işlem içeren örnek metod 
        static int SaveToFile<T>(IEnumerable<T> list) 
        { 
            //TODO: Çok büyük boyutlu bir veri içeriğinin dosyaya yazılması söz konusudur. Zaman alan bir işlem olarak düşünülebilir 
            return 1; 
        } 
        // Async Callback tekniğine göre SaveToFileDelegate<T> ile işaret edilen metod sonlandığında devreye girecek olan geri bildirim fonksiyonu 
        static void Callback(IAsyncResult asyncResult) 
        { 
            // EndInvoke çağrısı ile ilişkili olan delegate tipi yakalanır 
            var dlg = asyncResult.AsyncState as SaveToFileDelegate<object>; 
            // delegate tipi üzerinden EndInvoke çağrısı yapılarak SaveToFile metodunun sonucu alınır 
            int result = dlg.EndInvoke(asyncResult); 
            // Sonuç değerlendirilir 
        } 
    } 
}
```

Delegate tiplerinin kullanımı göz önüne alındığında, Polling, Callback, WaitHandle gibi modelleri destekleyebilen BeginInvoke ve EndInvoke metod çağırımlarının söz konusu olduğu bilinmektedir. BeginInvoke metodu IAsyncResult arayüzü (Interface) tipinden bir referans döndürmekte olup, program kod akışının izleyen satırdan devam edebilmesini sağlmaktadır. Pek tabi, EndInvoke metodu içerisinde de ilgili IAsyncResult arayüz referansından yararlanarak sonuçların alınması söz konusudur. Bu iki metod arasındaki çalışma süreci, ana sürece (varsayılan olarak uygulamanın Main Thread’ i olarak da düşünebiliriz) veya diğer süreçlere paralel olarak yürütülmektedir.

Dolayısıyla benzer bir yaklaşımı WCF servislerinde, asenkron hale getirilmek istenen operasyonlar için de düşünebiliriz. Gelin öncelikle senkron olarak uzun süren işlem içeren örnek bir WCF servisini geliştirelim. Bu amaçla aşağıdaki servis sözleşmesini (Service Contract) içeren bir servis uygulamasını göz önüne alabiliriz.

[![awcf_1](/assets/images/2012/awcf_1_thumb.png)](/assets/images/2012/awcf_1.png)

IProductService arayüz tipi (Interface);

```csharp
using System.ServiceModel;

namespace AzonServices 
{ 
    [ServiceContract] 
    public interface IProductService 
    { 
        [OperationContract] 
        int CreateProducts(int totalProductCount); 
    } 
}
```

ProductService sınıfı;

```csharp
using System; 
using System.Collections.Generic;

namespace AzonServices 
{ 
    public class ProductService 
       : IProductService 
    { 
        public int CreateProducts(int totalProductCount) 
        { 
            int createdProductCount = 0; 
            char[] classes = {'C', 'D', 'E', 'L', 'S'}; 
            List<Product> products = new List<Product>(); 
            Random randomizer = new Random();

            for (int i = 0; i < totalProductCount; i++) 
            { 
                Product newProduct = new Product 
                                         { 
                                             ProductId=i, 
                                             Name="PRD-"+i.ToString(), 
                                             ListPrice=randomizer.Next(1,100), 
                                             StockSize=randomizer.Next(50,500), 
                                             Class=classes[randomizer.Next(0,classes.Length)] 
                                         }; 
                products.Add(newProduct); 
                createdProductCount++; 
            } 
            //TODO: product listesinin veritabanın yazılma veya dosyaya kayıt edilme işlemi yapılacak 
            return createdProductCount; 
        } 
    } 
}
```

Product POCO sınıfı;

```csharp
namespace AzonServices 
{ 
    class Product 
    { 
        public char Class { get; set; } 
        public int StockSize { get; set; } 
        public int ListPrice { get; set; } 
        public string Name { get; set; } 
        public int ProductId { get; set; } 
    } 
}
```

CreateProducts metodu istemciden aldığı toplam miktara göre bir Product listesi üretmektedir. Test amaçlı olarak üretilen bu listenin içerisinde yer alan ürün bilgileri, rastgele değerlerden oluşmaktadır. İstemcinin vereceği maksimum ürün sayısına göre ilgili operasyonun uzun sürmesi olasıdır. İstemci açısından bakıldığında, söz konusu operasyonunun tamamlanana kadar uygulama içerisinde beklenmesine gerek yoktur. Bu, zaten istemci tarafında sahip olduğumuz ilgili servisi asenkron olarak çağırma yeteneğidir. Hatta servisi istemci tarafına eklerken Add Service Reference arabirimindeki ilgili opsiyon etkinleştirilerek servis operasyonlarının olay bazlı (Event Based) asenkron çağırım versiyonlarının üretilmesi sağlanabilmektedir.

> İstemci tarafında servis çağırımlarına ait asenkron operasyon desteğini etkinleştirmek için, Generate asynchronous operations özelliğinin işaretlenmiş olması gerekmektedir.

[![awcf_2](/assets/images/2012/awcf_2_thumb.png)](/assets/images/2012/awcf_2.png)

Sonuç olarak üretilen Async uzantılı asenkron çağırım metodu ve servis operasyon işleminin tamamlanması sonrası devreye girecek fonksiyonu işaret edecek olan olay (Event), kod tarafında değerlendirilebilir olacaktır.

[![awcf_3](/assets/images/2012/awcf_3_thumb.png)](/assets/images/2012/awcf_3.png)

Gayet güzel

![Wink](/assets/images/2012/smiley-wink.gif)

Buraya kadar ki kısımda zaten pek bir sıkıntı yok açıkçası. Ancak şöyle bir durum da söz konusu,

> Servise eş zamanlı olarak gelen çağrılarda ve tek bir servis örneğinin (Instance) oluşmasının tercih edildiği durumlarda, servis üzerindeki bu yük nasıl asenkronize edilebilir?

İşte yazımızın asıl konusu da budur.

Başlarda da belirttiğimiz gibi servis tarafında IAsyncResult arayüzü ve Task tiplerini kullanarak, eş zamanlı (Concurrent) olarak gelen çağrılarda, ilgili operasyonların servis tarafını gereksiz yere duraksatması engellenebilir. Bunun için IProductService servis sözleşmesini (Service Contract) ve ProductService sınıfını aşağıdaki gibi değiştirmemiz yeterli olacaktır.

Önce Class Diagram üzerinden ilgili değişikliklere bir bakalım.

[![awcf_4](/assets/images/2012/awcf_4_thumb.png)](/assets/images/2012/awcf_4.png)

IProductService arayüzünün yeni versiyonu;

```csharp
using System.ServiceModel; 
using System;

namespace AzonServices 
{ 
    [ServiceContract] 
    public interface IProductService 
    { 
        [OperationContract(AsyncPattern=true, Action="CreateProducts", Name="CreateProducts", ReplyAction ="CreateProductsReply")] 
        IAsyncResult BeginCreateProducts(int totalProductCount,AsyncCallback callback,object asyncState);

        int EndCreateProducts(IAsyncResult result); 
    } 
}
```

Servis sözleşmesinde Begin ve End ön ekleri ile başlayan iki metod yer almaktadır. Dikkat edilmesi gereken noktalardan birisi, EndCreateProducts metodunun OperationContract niteliği (Attribute) ile imzalanmamış oluşudur. Nitekim bu metod, BeginCreateProducts metodunun tamamlanması sonucu devreye giren metod olmakla birlikte, sadece servis tarafını ilgilendiren bir fonksiyondur. Dolayısıyla istemci tarafına açılmasına söz konusu değildir.

BeginCreateProducts metoduna ait OperationContract niteliğinde ise bazı özellikler set edilmiştir. Herşeyden önce ilgili operasyonun Asenkron desene uygun olarak çalışacağının belirtilmesi gerekmektedir. Bu amaçla AsyncPattern özelliğine true değeri verilmiştir. Diğer taraftan aksiyon, operasyonun istemci tarafından görünecek olan adı ve istemci tarafında verilecek olan cevaba ait Action bilgisi de ilgili özelliklerce set edilmiştir.

BeginCreateProducts metodu ve EndCreateProducts metodlarının şema yapılarına bakıldığında, temsilci (Delegate) tiplerinin BeginInvoke ve EndInvoke metodlarından farksız oldukları gözlemlenmektedir. (İsimlendirme standartdını bozmamak açısından da bu şekilde bir adlandırma tercih edilmelidir)

BeginCreateProducts metodu, ilk parametre olarak istemciden gelecek olan integer değeri almaktadır. Son iki parametre ise değişmez sırada olmalıdır. Bunlardan birisi Callback metodunu işaret edecek olan AsyncCallback temsilcisi iken, son parametre de EndCreateProducts metodu içerisinde BeginCreateProducts fonksiyonunda başlatılan Task örneğini yakalamak ve dolasıyıyla sonucunu almak üzere kullanılan object referansıdır.

ProductService sınıfının yeni versiyonu;

```csharp
using System; 
using System.Collections.Generic; 
using System.ServiceModel; 
using System.Threading.Tasks;

namespace AzonServices 
{ 
    [ServiceBehavior(InstanceContextMode= InstanceContextMode.Single, ConcurrencyMode= ConcurrencyMode.Multiple)] 
    public class ProductService 
        : IProductService 
    { 
        public IAsyncResult BeginCreateProducts(int totalProductCount, AsyncCallback callback, object asyncState) 
        { 
            var task = new Task<int>((s) => 
                                         { 
                                             int createdProductCount = 0; 
                                             char[] classes = {'C', 'D', 'E', 'L', 'S'}; 
                                             List<Product> products = new List<Product>(); 
                                             Random randomizer = new Random();

                                             for (int i = 0; i < totalProductCount; i++) 
                                             { 
                                                 Product newProduct = new Product 
                                                                          { 
                                                                              ProductId = i, 
                                                                              Name = "PRD-" + i.ToString(), 
                                                                              ListPrice = randomizer.Next(1, 100), 
                                                                              StockSize = randomizer.Next(50, 500), 
                                                                              Class = 
                                                                                  classes[ 
                                                                                      randomizer.Next(0, classes.Length) 
                                                                                  ] 
                                                                          }; 
                                                 products.Add(newProduct); 
                                                 createdProductCount++; 
                                             } 
                                             return createdProductCount; 
                                         }, asyncState);

            task.ContinueWith((t) => { callback(t); }); 
            task.Start();

            return task; 
        }

        public int EndCreateProducts(IAsyncResult result) 
        { 
            var task = (Task<int>) result; 
            return task.Result; 
        } 
    } 
}
```

ProductService tipinin ilk dikkat çeken noktalarından birisi, ServiceBehvaior niteliğinde set edilen özelliklerdir. Servisin bellek üzerinde tek bir örnek olarak oluşturulması belirlendikten sonra (InstanceContextMode.Single), servis operasyonlarında Async değeri true olanlara da eş zamanlı olarak erşilebileceği ifade edilmektedir (ConcurrencyMode.Multiple)

BeginCreateProducts metodunun içerisinde Task tipinden yararlanıldığı görülmektedir. Asenkron olarak yürütülmek istenen operasyona ait kod parçaları Task tipi örneklenirken ilgili isimsiz metod (Anonymous Method) içerisine yazılmıştır. Bu örnekte Task tipinin generic bir versiyonun kullanıldığı görülmektedir. Nitekim metod geriye int tipinden bir sonuç döndürecek şekilde tasarlanmıştır. Task tipinin örneklenmesi sırasında kullanılan asyncState nesne örneği EndCreateProducts metodu içerisinde yakalanacak olan Task nesne örneğine ait referans olacaktır.

İlerleyen satırlarda task nesne örneğinin tamamlanması sonucu callback değişkeni ile ifade edilen çalışma zamanı metodunun tetiklenmesi ve t isimli Task değişkeninin (ki bu t, task isimli değişkeni ifade etmektedir) ilgili geri bildirim metodunun (ki EndCreateProducts olmaktadır) IAsyncResult arayüzüne atanması sağlanır (ContinueWith kısmı). Start metodu ile bildiğiniz üzere ilgili task örneği başlatılmaktadır. Bundan sonraki aşama ise oldukçta basittir. EndCreateProducts metoduna gelecek olan IAsyncResult arayüzünden yararlanılarak çalışma zamanındaki Task referansı yakalanmakta ve Result özelliği ile çalışma sonucu üretilen tamsayı değeri geriye, bir başka deyişle itemci tarafına döndürülmektedir.

Biraz karmaşık gözüken bir desen olduğunun farkındayım

![Undecided](/assets/images/2012/smiley-undecided.gif)

Ancak kalıp olarak düşünüldüğünde pek çok servis operasyonuna kolayca entegre edilebilecek bir yapı olarak düşünülebilir. Elbette bu konuyu bir de,.Net Framework 4.5’ e entegre olarak gelen yeni async ve await anahtar kelimelerini göz önüne alarak değerlendirmekte yarar vardır. Bunu da ilerleyen zamanlarda incelemeyi planlamaktayım

![Wink](/assets/images/2012/smiley-wink.gif)

Böylece geldik bir yazımızın daha sonuna. Bir sonraki yazımızda görüşünceye dek hepinize mutlu günler dilerim.

[AzonServices.zip (67,73 kb)](/assets/files/2012/AzonServices.zip)