---
layout: post
title: "CallBack Tekniği ile Asenkron Metod Yürütmek"
date: 2005-09-09 12:00:00 +0300
description: "Çoğu zaman projelerimizde, çalışmakta olan uygulamaları uzun süreli olarak duraksatacak işlevlere yer veririz. Özellikle görsel tabanlı uygulamalarda veritabanlarına ait kapsamlı sorguların yer aldığı işlemlerde bu sorunla sıkça karşılaşılmaktadır. En büyük problem var sayılan olarak kod satırlarının senkron hareket etmesidir. Yani kodlar sırası geldikçe işleyen parçalar bütününden oluşmaktadır. Bu elbetteki uzun sürecek bir sorgunun cevapları alınmadan izleyen kod satırlarının işlememesi anlamına gelmektedir. Oysaki kodları asenkron olarak çalıştırma şansımızda mevcuttur. Eminim ki Ado.Net 2.0' da asenkron metod yürütme tekniklerini veya asenkron web servisi uygulamalarınının nasıl yazılacağını duymuşsunuzdur. Temel prensib hepsi için aynıdır. Merkezde IAsyncResult arayüzü tipinden bir nesnenin kullanıldığı temsilci (delegate) tabanlı modeller söz konusudur."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - ado-net
  - web-service
  - async-await
  - threading
  - delegates
  - dataset
---
# CallBack Tekniği ile Asenkron Metod Yürütmek
Değerli Okurlarım Merhabalar,

Çoğu zaman projelerimizde, çalışmakta olan uygulamaları uzun süreli olarak duraksatacak işlevlere yer veririz. Özellikle görsel tabanlı uygulamalarda veritabanlarına ait kapsamlı sorguların yer aldığı işlemlerde bu sorunla sıkça karşılaşılmaktadır. En büyük problem var sayılan olarak kod satırlarının senkron hareket etmesidir. Yani kodlar sırası geldikçe işleyen parçalar bütününden oluşmaktadır. Bu elbetteki uzun sürecek bir sorgunun cevapları alınmadan izleyen kod satırlarının işlememesi anlamına gelmektedir. Oysaki kodları asenkron olarak çalıştırma şansımızda mevcuttur. Eminim ki Ado.Net 2.0' da asenkron metod yürütme tekniklerini veya asenkron web servisi uygulamalarınının nasıl yazılacağını duymuşsunuzdur. Temel prensib hepsi için aynıdır. Merkezde IAsyncResult arayüzü tipinden bir nesnenin kullanıldığı temsilci (delegate) tabanlı modeller söz konusudur.

Bir temsilci her hangi bir metodun başlanıç adresini işaret eden bir tip (type) tir. Metodların başlangıç adreslerini işaret eden bir temsilci çalışma zamanında polimorfik bir yapıya sahiptir. Bu sayede yeri geldiği zaman kendi desenine uygun her hangi bir metodun yürütülmesini sağlayabilir. İşte bu felsefeden yola çıkarak çalışma zamanında asenkron olarak yürütülecek metodlarda oluşturulabilir. Bu noktada devreye IAsyncResult arayüzü (interface) girer. Temel olarak asenkron olarak çalıştırılmak istenen metod bir temsilci vasıtasıyla yürürlüğe sokulur. Bu anda ortama IAsyncResult tipinden bir arayüz nesnesi döner. Anlık olan bu işlem nedeni ile uygulamanın geri kalan kod satırları duraksamadan işlemeye devam eder. Ancak bu sırada ilgili temsilcinin başlattığı işlemler ayrı bir thread (iş parçacığı) içerisinde yürütülmeye devam etmektedir. Peki yürütülen thread sonlandığında, üretilen sonuçlar ortama nasıl alınacaktır? İşte burada çeşitli teknikler kullanılabilir. Bizim bu makalede işleyeceğimiz olan teknik Callback modelidir.

Herşeyden önce asenkron yürütme tekniğinin kalbi olan temsilcilerin MSIL (Microsoft Intermediate Language) koduna bakmak ve anlamak gerekir. Söz gelimi aşağıdaki gibi bir temsilci tipi tanımladığımızı düşünelim. Bu son derece yalın bir delegate tanımlamasıdır.

```csharp
public delegate void Temsilci();
```

Bu temsilciyi kullandığımız her hangi bir uygulamaya ait assembly'ı ILDASM (Intermeidate Language DisAssembly) aracı yardımıyla açarsak IL kodu içerisinde temsilci nesnemize ait iki metod tanımlı olduğunu görürüz.

![mk135_1.gif](/assets/images/2005/mk135_1.gif)

BeginInvoke ve EndInvoke metodları tamamıyla asenkron işlemler için geliştirilmiştir. BeginInvoke metodu, temsilcimizin çalışma zamanında işaret ettiği metodu yürürlüğe sokmak ve o anda ortama bir IAsyncResult arayüzü nesne örneği göndermek ile yükümlüdür. BeginInvoke metodu, temsilci nesnesinin işaret etmiş olduğu metodu ayrı bir thread içerisine atar. EndInvoke metodu ise dikkat edecek olursanız parametre olarak IAsyncResult arayüzü tipinden bir nesne örneğini alır. İşte bu nesne, BeginInvoke ile başlatılan process'ten sorumlu olan nesnedir. Dolayısıyla EndInvoke metodu içerisinden, asenkron metodun yer aldığı process'e ait sonuçlar yakalanıp ortama aktarılabilir.

Callback modelinde BeginInvoke metodunun AsnycCallback temsilci tipinden olan parametresi kullanılır. Bu nesne asenkron olarak çalıştırılan metoda ait işlemler sonlandığında otomatik olarak devreye girecek metodu işaret eden temsilci tipinden başka bir şey değildir. En genel kullanımda bu AsyncCallback temsilcisi EndInvoke metodunu içeren başka bir metodu temsil eder.

![dikkat.gif](/assets/images/2005/dikkat.gif)
Asenkron Callback modelinde, işlemlerin sonuçlanmasının hemen ardından devreye girecek olan metod Callback metodu olarak adlandırılır. Bu metodu çalışma zamanında işaret edebilmek için özel AsyncCallback temsilci (delegate) tipinden faydalanılır.

Şimdi en basit haliyle Callback modelini masaya yatıralım. Basit olarak aşağıdaki Console uygulamasını geliştireceğiz.

```csharp
using System;

namespace UsingAsyncCallback
{
    public delegate void Temsilci();

    class Yurutucu
    {
        public void Calistir(Temsilci t)
        {
            t.BeginInvoke(new AsyncCallback(SonuclariAl),t);
        }

        public void SonuclariAl(IAsyncResult ia)
        {
            Temsilci t=(Temsilci)ia.AsyncState;
            t.EndInvoke(ia);
        }

        public void Islemler()
        {
            Console.WriteLine("Async yürütülecek metod");
        }
    }

    class AnaProgram
    {
        [STAThread]
        static void Main(string[] args)
        {
            Yurutucu yrtc=new Yurutucu();
        
            #region Asenkron kullanıldığında

            Temsilci t=new Temsilci(yrtc.Islemler);
            yrtc.Calistir(t);
            Console.WriteLine("Diğer kod satırları...");

            #endregion

            #region Asenkron kullanılmadan

            // yrtc.Metod();
            // Console.WriteLine("Diğer kod satırları...");
        
            #endregion

            Console.ReadLine();
        }
    }
}
```

İlk olarak Temsilci isimli bir delegate tipi tanımladık. Bu tip çalışma zamanında her hangi bir parametre almayan ve geri dönüş tipi bulunmayan (void) metodları işaret edebilecek cinstedir. Asenkron işlemlerin toplu olarak yer aldığı Yurutucu isimli bir sınıfımız var. Bu sınıf içerisinde hem asenkron metodun başlatılmasını sağlayacağız, hem de işlmelerin sonlanmasının ardından devreye girecek Callback metodumuzu işleteceğiz.

Temsilcimizin BeginInvoke ve EndInvoke metodlarını çalıştıracak iki metodumuz var. Islemler isimli metodumuz asenkron olarak yürütmeyi düşündüğümüz metod olacak. Calistir isimli metodumuz Temsilci tipinden bir parametre almakta ve içeride bu nesne üzerinden BeginInvoke metodunu çağırmaktadır. BeginInvoke metodumuzun AsyncCallback temsilcisi tipinden olan parametresi Islemler isimli metodumuzu işaret edecek şekilde tanımlanmıştır. Bu şu anlama gelmektedir. Calistir isimli metod uygulandığında BeginInvoke, temsilcinin işaret ettiğim metodu asenkron olarak yürürlüğe sokacaktır. Asenkron olarak çalışan metod sonlandığında ise AsyncCallback nesnesinin işaret ettiği SonuclariAl isimli metod otomatik olarak devreye girecektir.

SonuclariAl isimli metodumuz geriye bir değer döndürmez ve sadece IAsyncResult arayüzü tipinden bir parametre alır. Bunun sebebi AsyncCallback temsilcisinin bu tipteki metodları işaret edebilecek olmasıdır. SonuclariAl isimli metodumuz içerisinde EndInvoke metodu ile asenkron işlemlerin sonucu ortama alınmak istenmektedir. Bunu sağlayabilmek için IAsyncResult tipinden parametre Temsilci tipine dönüştürülür ki bu sayede temsilci üzerinden BeginInvoke çağırılabilir. Böylece o ana kadar çalışmış olan işlemlerin sonucunu alabilecek konuma gelmiş oluruz.

Model ilk bakışta karışık gözükebilir. Ancak temel noktaları anladığınızda sorun kalmayacaktır. Biz temsilcimizin BeginInvoke metodu ve EndInvoke metodunun olduğunu biliyoruz. Bu metodları kullanırken AsyncCallback temsilci tipi ile Callback fonksiyonu olarak EndInvoke metodunu çağırdığımız bir metodu işaret ediyoruz. Burada dikkat etmemiz gereken AsyncCallback temsilcisinin geri dönüş tipi olmayan ve IAsyncResult arayüzü tipinden nesneleri parametre olarak alan metodları işaret edebilecek olmasıdır. Bundan sonra tek yapmamız gereken çalışma zamanında temsilcimizi işaret edeceği asenkron metodu gösterecek şekilde oluşturmak ve BeginInvoke yapısını barındırdan metodu yürürlüğe sokmak. Uygulamamızı yukarıdaki hali ile çalıştırdığımızda aşağıdakine benzer bir ekran görüntüsü elde ederiz.

![mk135_3.gif](/assets/images/2005/mk135_3.gif)

Şimdi burada anlamamız gereken nokta şudur. Biz temsilcimiz ile işaret ettiğimiz metodu çalıştırdıktan sonra normal şartlar altında bu metod sonlanıncaya kadar kodun beklemesi ve kod tamamlandıktan sonra kalan satırların işlemesi gerekirdi. Oysaki biz metodumuzu asenkron olarak yürüttüğümüzden, temsilci nesnemizin işaret ettiği metodu çalıştırdığımız kod satırından sonraki satırlar daha önceden çalışabilmiştir. Bu ayırımı anlamak çok önemlidir. Olayı daha iyi kavrayabilmek için Main metodundaki kodları aşağıdaki gibi düzenleyelim.

```csharp
Yurutucu yrtc=new Yurutucu();

#region Asenkron kullanıldığında

// Temsilci t=new Temsilci(yrtc.Islemler);
// yrtc.Calistir(t);
// Console.WriteLine("Diğer kod satırları...");

#endregion

#region Asenkron kullanılmadan

yrtc.Islemler();
Console.WriteLine("Diğer kod satırları...");

#endregion

Console.ReadLine();
```

Uygulamayı tekrar çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk135_4.gif](/assets/images/2005/mk135_4.gif)

Gördüğünüz gibi önce Islemler isimli metodumuz çalıştı. Bu metodun çalışması bittikten sonra kalan kod satırlarından uygulama çalışmaya devam etti. Bunun sebebi işleyişin senkron olarak yürümesidir.

Callback modeli özellikle veri çekme işlemi uzun süren sorguların yer aldığı uygulamalarda oldukça işe yaramaktadır. Örneğin AdventureWorks2000 veritabanı üzerinde aşağıdaki gibi bir sorgunun kullanıldığı bir uygulama geliştirdiğimizi düşünelim.

```text
SELECT * FROM Customer 
    INNER JOIN CustomerAddress ON Customer.CustomerID = CustomerAddress.CustomerID 
    INNER JOIN Address ON CustomerAddress.AddressID = Address.AddressID 
    INNER JOIN SalesPerson ON Customer.SalesPersonID = SalesPerson.SalesPersonID 
    INNER JOIN SalesPersonQuotaHistory ON SalesPerson.SalesPersonID = SalesPersonQuotaHistory.SalesPersonID
```

Gerçek hayatta bu ve bunun gibi daha çok zaman alacak sorguları sıkça kullanıyoruz. Yukarıdaki sorgu her ne kadar bizim için pek bir şey ifade etmesede örnek uygulamamızda Callback modelini nasıl kullanacağımıza dair uygun bir gecikme süresi sağlayacak yapıdadır. Uygulamamızın deseni yukarıdaki ile neredeyse aynı olacak. Ancak bu kez önemli bir fark var. Bu da asenkron olarak yürütülecek metodumuzun DataSet döndüren ve string parametre alan bir yapıda olması. Dolayısıyla bu metodu işare edecek temsilci nesnemizde aşağıdaki gibi olmalıdır.

```csharp
public delegate DataSet Temsilci(string sorguCumlesi);
```

Eğer ILDASM aracı ile bu temsilinin MSIL koduna bakacak olursak BeginInvoke ve EndInvoke metodlarının bir önceki örneğimize nazaran biraz daha farklı oluşturulduğunu görürüz.

![mk135_5.gif](/assets/images/2005/mk135_5.gif)

Dikkat ederseniz temsilcimizin string parametresi BeginInvoke metodunun ilk parametresidir. Ayrıca temsilcimizin dönüş tipide EndInvoke metodunun dönüş tipi olmuştur (DataSet).

![dikkat.gif](/assets/images/2005/dikkat.gif)
Asenkron yürütülecek olan metodlarımız geri dönüş tipine ve parametrelere sahip ise, BeginInvoke ve EndInvoke metodlarıda bu yapıya göre şekillenecektir. EndInvoke asenkron metodun dönüş tipinden değer döndürürken, BeginInvoke asenkron metodda tanımlanan parametre sayısı kadar parametreyi ek olarak alacaktır.

Dolayısıyla hem çalıştırıcı metodumuzu hem de Callback metodumuzu bu yapıya uygun olarak tasarlamalıyız. İşte örnek kodlarımız.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace UsingAsyncCallback2
{
    public delegate DataSet Temsilci(string sorguCumlesi);

    class Yurutucu
    {
        public DataSet ds;

        public void Baslat(Temsilci t,string sorgu)
        {
            t.BeginInvoke(sorgu,new AsyncCallback(Bitir),t);
        }

        public void Bitir(IAsyncResult ia)
        {
            Temsilci t=(Temsilci)ia.AsyncState;
            ds=t.EndInvoke(ia);
            Console.WriteLine(ds.Tables[0].Rows[1][5]+" "+ds.Tables[0].Rows[1][6]+" "+ds.Tables[0].Rows[1][7]);
        }

        public DataSet SonuclariAl(string sorgu)
        {
            SqlConnection con=new SqlConnection("data source=localhost;database=AdventureWorks2000;user id=sa");
            SqlDataAdapter da=new SqlDataAdapter(sorgu,con);
            DataSet dsSonucKumesi=new DataSet();
            da.Fill(dsSonucKumesi);
            return dsSonucKumesi;
        }
    }

    class AnaProgram
    {
        [STAThread]
        static void Main(string[] args)
        {
            Yurutucu yrtc=new Yurutucu();
            string sorgu=@"SELECT * FROM Customer INNER JOIN CustomerAddress ON Customer.CustomerID =         CustomerAddress.CustomerID INNER JOIN Address ON CustomerAddress.AddressID = Address.AddressID INNER JOIN SalesPerson ON Customer.SalesPersonID = SalesPerson.SalesPersonID INNER JOIN SalesPersonQuotaHistory ON SalesPerson.SalesPersonID = SalesPersonQuotaHistory.SalesPersonID";
            Temsilci t=new Temsilci(yrtc.SonuclariAl);
            yrtc.Baslat(t,sorgu);
            for(int i=1;i<3000;i++)
            {
                Console.Write("."); 
            }
            Console.ReadLine();
        }
    }
}
```

Uygulamamızı çalıştırdığımızda yaklaşık olarak aşağıdakine benzer bir görüntü ile karşılaşırız.

![mk135_2.gif](/assets/images/2005/mk135_2.gif)

Dikkat ederseniz for döngüsü başlamadan önce asenkron olarak yürütmek istediğimiz metodumuz olan SonuclarıAl temsilci nesnemiz yardımıyla çalıştırılımıştır. Bu işlemin ardından BeginInvoke metodumuz asenkron olarak çalışan SonuclariAl metodunu ayrı bir iş parçasına bir IAsyncResult arayüzü nesnesi sorumluluğunda devretmiştir. BeginInvoke metodunu çalıştırdığımızda Callback metodumuzuda bildirdiğimizden sorgu sonuçlandığı anda EndInvoke metodunun yer aldığı Bitir isimli metod devreye girecektir. Bu metod içerisinde tamamlanan asenkron process'ten sorumlu IAsyncResult nesne örneği kullanılarak Temsilci nesnemiz elde edilmiş ve bu nesne üzerinden EndInvoke çağırılarak sorgu sonucu elde edilen DataSet alınmıştır. Dikkat edin BeginInvoke metodu geriye bir DataSet nesne örneği döndürmektedir. Son olarak örnek olması açısından veri kümesinin ilk satırına ait bir kaç alan bilgisi ekrana yazdırılır.

Burada önemli olan nokta, metod çalışmaya başladıktan sonra, metodun içerdiği sorgunun sonlanması beklenmeden for döngüsünün devreye girmesidir. For döngüsü işleyişini sürdürürken, asenkron metodumuz tamamlandığında sonuçlar ekrana yazdırılmış ve for döngüsü kaldığı yerden işleyişine devam etmiştir. İşte asenkron Callback modeli.

Callback modeli yardımıyla asenkron işlemlerin gerçekleştirilmesi özellikle profesyonel uygulamalarda büyük önem arz etmektedir. Özellikle uygulamaların uzun süre duraksamasını (donmasını) engelleyebileceğimiz bir yoldur. Ancak asenkron modellerde birbirlerini etkileyebilecek işlemler varsa dikkatli davranılmalıdır. Aksi takdirde process'lerin sonsuz döngülere girerek asılı kalmasına neden olabiliriz.

Böylece geldik bir makalemizin daha sonuna bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Kodlar İçin Tıklayınız.](/assets/files/2005/AsyncCallback.rar)