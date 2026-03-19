---
title: "Ado.Net 2.0 ve Sql Komutlarını Asenkron Olarak Yürütmek - 2"
pubDate: 2004-09-25 12:00:00
categories:
  - Ado.net 2.0
tags:
  - Ado.net 2.0
  - Bash
  - C#
  - .NET
  - ADO.NET
  - Async/Await
  - Delegates
  - Generics
  - Visual Studio
---

# Ado.Net 2.0 ve Sql Komutlarını Asenkron Olarak Yürütmek - 2
Değerli Okurlarım Merhabalar,

Hatırlayacağınız gibi bir önceki makalemizde, sql komutlarının asenkron olarak yürütülmesi için kullanılan tekniklerden birisi olan polling modelini incelemiştik. Polling modeli basit olmakla birlikte, iş yükü fazla olan hacimli sql komutlarının asenkron olarak çalıştırılmasında çok fazla tercih edilmemelidir. Bu tip sorguların yer aldığı asenkron yürütmelerde, CallBack veya Wait modellerini kullanmak verimliği arttırıcı etkenlerdir. Bu makalemizde CallBack modelini kısaca incelemeye çalışacağız.

CallBack modeli anafikir olarak, asenkron olarak çalışan sql komutlarının işleyişlerinin sona erdiği noktalarda yürürlüğe giren metodları bünyesinde barındıran bir tekniktir. Bu tekniğe göre, asenkron olarak yürütülecek sql komutlarını taşıyan SqlCommand nesneleri yine bilinen Begin... metodları ile çalıştırılırlar. Ancak bu kez, SqlCommand nesnesine ait Begin metodlarının aşağıdaki tabloda belirtilen aşırı yüklenmiş versiyonları kullanılır.

CallBack modelinde kullanılan SqlCommand.Begin... metodları

public IAsyncResult BeginExecuteNonQuery (AsyncCallback callback, object stateObjcet);

public IAsyncResult BeginExecuteReader (AsyncCallback callback, object stateObjcet);

public IAsyncResult BeginExecuteXmlReader (AsyncCallback callback, object stateObjcet);

Burada görüldüğü gibi her üç metod da, iki parametre almaktadır. İlk parametre AsyncCallback temsilcisi tipindendir. Bu parametre yardımıyla, yürütülecek olan sql komutları tamamlandığında çalıştırılacak olan metod işaret edilir. Bu, static olan, void geri dönüş değerine sahip ve yanlızca IAsyncResult tipinde bir nesne örneğini parametre olarak alan bir metod olmalıdır. Bir başka deyişle Begin metodu ile çalıştırılan sql sorguları sonlandığında hangi metodun çalıştırılacağı buradaki temsilci (delegate) yardımıyla belirlenmiş olunur.

İkinci parametre ise kullanıcı tarafından belirtilebilen object tipinden bir nesnedir. Çoğunlukla, CallBack metodu içine, asenkron olarak çalışan sql komutunun sahibi olan SqlCommand nesnelerini aktarmak amacıyla kullanılmaktadır. Polling modelinde olduğu gibi burada da Begin metodlarının geriye dönüş değerleri, çalışan asenkron sorgudan sorumlu olan IAsyncResult arayüzü tipinden nesne örnekleridir.

Bu kısa açıklamalardan sonra dilerseniz, CallBack modelinin nasıl uygulandığını gösteren basit bir örnek geliştirelim. Bu amaçla Visual Studio.Net 2005' te aşağıdaki kodlara sahip olan bir Console uygulaması oluşturalım.

```bash
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

#endregion

namespace CallBackModel
{
    class Program
    {
        public static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection("data source=localhost;initial catalog=AdventureWorks;integrated security=SSPI;MultipleActiveResultSets=true;async=true");
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Production.Product SET ListPrice=ListPrice*1.15", con);
            SqlCommand cmd2 = new SqlCommand("SELECT * From Person.Address", con);
            IAsyncResult res = cmd.BeginExecuteNonQuery(new AsyncCallback(UPDATE_OK), cmd);
            IAsyncResult res2 = cmd2.BeginExecuteReader(new AsyncCallback(SELECT_OK), cmd2);
            Console.WriteLine("SORGULAR ÇALIŞIYOR...");
            Console.ReadLine();
            con.Close();
        }

        public static void UPDATE_OK(IAsyncResult r)
        {
            SqlCommand komut = (SqlCommand)r.AsyncState;
            int sonuc=komut.EndExecuteNonQuery(r);
            Console.WriteLine(sonuc + " SATIR GÜNCELLENDİ...");
        }
    
        public static void SELECT_OK(IAsyncResult r)
        {
            SqlCommand komut = (SqlCommand)r.AsyncState;
            SqlDataReader dr = komut.EndExecuteReader(r);
            dr.Read();
            Console.WriteLine(dr[1] + " " + dr[2]);
        }
    }
}
```

Uygulamayı çalıştırmadan önce kısaca neler yaptığımıza daha yakında bakmakta fayda var. Bu örneğimizde, Yukon üzerinde yer alan AdventureWorks veritabanına bağlandık. Amacımız iki sql sorgusunu asenkron olarak çalıştırmak ve bu sorgular işlerken uygulama ortamında izleyen kod satırlarını yürütülebilmesini sağlamak. SqlCommand nesnelerimizide yarattıktan sonra sıra bu komutları ilgili Begin... metodları ile çalıştırmaya geliyor.

```csharp
IAsyncResult res = cmd.BeginExecuteNonQuery(new AsyncCallback(UPDATE_OK), cmd);
IAsyncResult res2 = cmd2.BeginExecuteReader(new AsyncCallback(SELECT_OK), cmd2);
```

Burada bizim için en önemli noktalar parametrelerdir. Her iki metodun ilk parametresi aslında AsyncCallback temsilcisi tipindendir. Nitekim;

```csharp
new AsyncCallback(UPDATE_OK)
```

tanımlaması ile aslında UPDATE_OK isimli metodu işaret eden AsyncCallback tipinden bir temsilci tanımlamış olunmaktadır. Bu temsilcinin yaptığı iş sadece, IAsyncResult nesne örneğinin sorumluluğunda olan sql komutlarının yürütülüşü tamamlandığında, derhal çalıştırılacak olan metodun hangi metod olacağına karar vermektir.

İkinci paramtremiz ise, Begin metodunun sahibi olan SqlCommand nesnesidir. Bu nesneye, UPDATE_OK metodu içinden erişilebiliriz. Eğer böyle bir erişim söz konusu olmasaydı, CallBack metodunun içinden sql sorgusunun sahibi olan SqlCommand nesnesine erişmekte sorunlar yaşayabilirdik. Nitekim CallBack metodu dikkat edeceğiniz gibi static olmak zorundadır ve static bir metod içinde static olmayan üyeler erişmekte sorun yaşamaktayızdır. Tüm bunlar AsyncCallback temsilcisinin işaret edebileceği metodun yapısı ile ilgilidir.

```csharp
public static void UPDATE_OK(IAsyncResult r)
{
      SqlCommand komut = (SqlCommand)r.AsyncState;
      int sonuc=komut.EndExecuteNonQuery(r);
      Console.WriteLine(sonuc + " SATIR GÜNCELLENDİ...");
}
```

Bu metod dikkat edecek olursanız, IAsyncResult tipinden bir nesneyi parametre olarak almaktadır. Buraya geçirilen bu parametre, Begin metodu ile oluşturulan IAsyncResult nesne örneğidir. Dolayısıyla, sql komutunun işleyişi tamamlandığında devreye girecek olan bu metod içinden, IAsyncResult nesne örneği yardımıyla sonuç kümelerini elde edebilmemiz mümkün olabilmektedir. Bir başka deyişle, End... metodunu buraya aktarılan IAsyncResult nesne örneği üzerinden çağırabiliriz. Diğer yandan, sql komutunun sahibi olan SqlCommand nesnesini metod içinde kullanabilmek için,

```csharp
SqlCommand komut = (SqlCommand)r.AsyncState;
```

satırı kullanılmıştır. Bu satırda dikkat ederseniz, IAsyncResult nesnesinin AsyncState özelliğinden yararlanılmıştır. Bu özellik, asenkron yürütme operasyonunda görev olan SqlCommand nesnesini elde etmemizi sağlar. Tabiki özelliğin tanımlanışı gereği, geri dönüş değeri object tipinde olduğundan burada bir cast işlemi uygulanmıştır. Yani object nesneyi SqlCommand tipine çevirmemiz gerekmektedir.

Son adımda ise, EndExecuteNonQuery metodu çağırılarak, sql komutunun sonuçları elde edilmiştir. Bu metod için kullanılan desen yapısı CallBack metodu olan SELECT_OK içinde geçerlidir. Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk97_1.gif](images/mk97_1.gif)

Şekil 1. Uygulamanın çalışması sonucu.

Dikkat edecek olursanız, biz CallBack tekniğini uygulamaya başladığımız Begin... çağrılarından sonraki kodlar, sql sorgularımızın tamamlanmasından önce çalışmıştır.

Bu modeli, bir önceki makalemizde incelediğimiz polling modeli ile karşılaştırdığımızda ilk göze çarpan, sürekli olarak bir kontrol yapmayışımızdır. Hatırlayacağınız gibi polling modelinde, IAsyncResult nesnesinin IsCompleted özelliği ile, çalışan sql sorgularının tamamlanıp tamamlanmadığı kontrol edilmektedir. Bu her ne kadar basit ve fazla zaman almayan sorgular için kullanışlı olsada, daha büyük ölçekli sorgularda, çalışan asenkron prosesin sürekli olarak kontrol edilmeye çalışılması çok anlamlı olmayacak ve en önemlisi zaman kaybında neden olacaktır. Bu nedenlede bu tip yoğun sorgularda genellikle CallBack veya Wait modellerinden birisi tercih edilir. CallBack modelinin anatomisini zihnimizde daha iyi canlandırabilmek amacıyla aşağıdaki şekli de göz önüne alabiliriz.

![mk97_2.gif](images/mk97_2.gif)

Şekil 2. CallBack modelinin anatomisi.

Görüldüğü gibi aslında, asenkron olarak çalışacak sql sorguları tamamlandığında devreye giren CallBack metodları, uygulama ortamındaki satırlardan tamamen bağımsız olarak çalışan yapılardır. Sql komutları sonlandığında anında devreye girerek sonuçların elde edilebilmesini sağlayan ve hatta başka işlemlerin gerçekleştirilebileceği kod bloklarını kapsüllememize imkan vermektedirler. CallBack yapısının biraz daha gelişmiş bir versiyonu olan Wait modelide asenkron sql komutları yürütülmesinde kullanılan tekniklerdendir. Bir sonraki makalemizde bu konuya değinmeye çalışacağız. Tekrar görüşünceye dek hepinize mutlu günler dilerim.
