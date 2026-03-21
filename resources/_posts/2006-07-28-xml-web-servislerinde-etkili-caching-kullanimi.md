---
layout: post
title: "Xml Web Servislerinde Etkili Caching Kullanımı"
date: 2006-07-28 12:00:00 +0300
categories:
  - xml-web-services
tags:
  - xml-web-service
  - cache
  - caching
  - context.cache
---
Ön-bellekleme (Caching) işlemleri web uygulamaları için ne kadar önemli ise Xml Web Servisleri içinde aynı durum geçerlidir. Ön-bellekleme sistemi sayesinde web uygulamalarının kullanıcıya cevap verme sürelerinin kısaltılması hedeflenmiştir. Bu da doğal olarak uygulamanın performansını arttırıcı bir etkendir. Kaldıki istekler önbellekten (cache) karşılandığı için, arka tarafta yapılan pek çok süreç atlanmaktadır. Dolayısıyla database işlemleri gibi maliyeti yüksek olan süreçlerin belirli kriterlere göre çalıştırılması ve kullanıcının istediği sonuçların en hızlı şekilde verilebilmesi ön-bellekleme sisteminin getirilerinden sadece birisidir. Web servisleride, web uygulamaları gibi 80 numaralı port üzerinden hizmet verdiklerinden ön-bellekleme yetilerine sahiptir. Web uygulamarında kullanılan ön-bellekleme mantığı ile web servislerinde kullanılan birbirlerine oldukça yakındır. Ancak bir takım farklılıklarda mevcuttur.

Bildiğiniz gibi, web uygulamalarında ön-bellekleme (caching) sistemi Output Caching ve Data Caching olmak üzere iki ana kategoriye ayırlmaktadır. Bir web uygulamasında Output Caching sayfa bazında veya user control bazında kullanılabilir. Böylece bir web sayfasının tamamının yada onun küçük bir parçasının ön-bellekte tutulması sağlanabilir. Oysaki aynı durum Web Servislerinde biraz daha farklıdır. Nitekim, web servislerinin kullanıcı ile etkileşimde olan bir arayüzü bulunmamaktadır. Buda kontrol veya sayfa bazında ön-bellekleme işleminin yapılamayacağı anlamına gelmektedir. Bir web servisi söz konusu olduğunda ön belleğe (cache) alınabilecek olan içerik bu servise ait metodların döndüreceği sonuç kümelerinden başka bir şey olmayacaktır. Dolayısıla web servislerinde metod bazında ön-belleklemenin yapılabileceğini söyleyebiliriz. Bunun dışında, web uygulamlarında sıkça kullanılan Data Caching tekniği web servisleri içinde geçerlidir. Kaldıki Data Caching tekniğinde kesin yaşam süresi (absolute expire time), hareketli yaşam süresi (floating expire time), dosya veya tablo bağımlılığı (cache dependency) gibi seçeneklerde mevcuttur. Bu seçenekler bir web servisi içerisinde etkin bir şekilde uygulandığında kullanıcılara hızlı cevap verebilmenin dışında, var olan sistem kaynaklarınıda daha etkili kullanabilme olanaklarına sahip oluruz. Bir web metodun döndüreceği sonuç kümesini ön-belleğe taşımak için yapılması gereken, WebMethod niteliğini aşağıdaki gibi CacheDuration özelliği ile kullanmaktır.

```csharp
[WebMethod(CacheDuration = 180)]
```

Bu niteliğin uygulandığı web metodun döndüreceği sonuçlar 180 saniye (3 dakika) süreyle ön bellekte tutulacaktır. Bu oldukça kullanışlı bir özelliktir. Ancak asıl dikkat edilmesi gereken nokta, parametre bağımlı dönüşlerin söz konusu olduğu web metodlarında ortaya çıkmaktadır. Nitekim web metodlarında parametreler söz konusu olduğundan parametre değerleri için ayrı ayrı ön-bellekleme işlemleri söz konusu olmaktadır. Örneğin, Sql Server 2005 üzerinde yer alan örnek AdventureWorks veritabanındanki Product tablosunun verilerini, ProductSubCategoryID değerine göre farklı veri kümeleri şeklinde sunabilecek bir web metodumuz olduğunu düşünelim. Normal bir web uygulamasında bu tip parametrik fonksiyonellikler söz konusu olduğunda genellikle OutputCache direktifinin VaryByParam niteliğinden faydalanılır. Oysaki web servisi metodlarında bu tarz bir kullanım söz konusu değildir. Web servisleri parametre bağımlı ön-bellekleme söz konusu olduğunda daha akılcı bir yaklaşım sergilyerek, metodun parametresine (parametrelerine) göre farklı ön-bellek alanları oluşturur. Böylece CacheDuration niteliği ile belirtilen ön-bellekleme süreleri içerisinde aynı parametrik değeri talep eden kulllanıcılar için ön-bellekte tutulan o parametreye ait görüntüler cevap olarak istemcilere gönderilir. Bunu daha iyi anlayabilmek için aşağıdaki kod parçasını göz önüne alalım.

```csharp
[WebMethod(Description = "Alt kategoriye göre ürünler", CacheDuration = 180)]
public DataSet GetProductsBySubCategory(int subCatId)
{
    using (SqlConnection con = new SqlConnection("data source=manchester;database=AdventureWorks;integrated security=SSPI"))
    {
        SqlCommand cmd=new SqlCommand("Select ProductID,Name,ListPrice,Size,StandardCost,ProductSubCategoryID From Production.Product Where ProductSubCategoryID=@SubCatId",con);
        cmd.Parameters.AddWithValue("@SubCatId",subCatId);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        da.Fill(ds);
        return ds;
    }
}
```

Bu örnek web metodu, parametre olarak Product tablosundaki ProductSubCategoryID alanı için kullanılacak bir parametre almaktadır. Talep edilen içeriğin ön bellekte tutulma süresi 180 saniye (3 dakika) olarak belirtilmiştir. Şimdi bu metoda belirli aralıklar ile 4 farklı talep geldiğini düşünelim. İlk ve ikinci taleplerde sırasıyla subCatId parametresine 1 ve 5 değerlerinin aktarıldığını düşünelim. Yani iki farklı kullanıcı farklı zamanlarda belirtilen kategorilerdeki ürünleri çekmek istesin. İlk talep sonrasında, yukarıdaki web metodunda yer alan kodlar 1 değeri için çalıştırılacaktır. Yani SqlConnection bağlantısı açılacak, ilgili SqlCommand oluşturulup bir SqlDataAdapter yardımıyla sonuç kümesi bir DataSet nesnesine aktarılacak ve geriye döndürülerek kullanıcıya cevap verilecektir. İşte bu andan itibaren oluşturulan sonuç kümesi ön belleğede atılır ve 180 saniye süreyle burada tutulur. Dolayısıyla aynı subCatId değeri için bu 180 saniyelik zaman dilimi içerisinde gelecek olan herhanbir talep, yukarıdaki kodlar çalıştırılmadan ön-bellekte tutulan veri içeriğinden döndürülecektir. İkinci ve farklı bir subCatId değeri için gelen talep içinde aynı süreç söz konusu olacaktır ve ön-bellekte ayrı bir alan açılarak ve CacheDuration süresi içerisinde gelen taleplerin cevapları buradan karşılanacaktır. Aşağıdaki çizelgede bir zaman dilimi içerisinde iki farklı subCatId için gelen taleplerin ön-bellekleme sistemince nasıl ele alınacağı gösterilmeye çalışılmıştır.

![mk169_1.gif](/assets/images/2006/mk169_1.gif)

Özellikle parametre bağımlı ön-bellekleme işlemlerinde web servisleri için dikkat edilmesi gereken bir nokta vardır. Web servisi isteğimiz dışında parametre bağımlı ön-bellekleme yapmaktadır. Bu otomatik gelişim zaman zaman iyi sonuçlar versede bazı durumlarda kullanılmamalıdır. Otomatik karar sistemi özellikle verinin çok yüksek boyutlara erişmediği hallerde oldukça faydalı bir yöntemdir. Ancak veri kümesinin boyutunun büyük olması, ön-bellekte tutulan veri kümelerininde, gelen sayısız talep sonucu belleği inanılmaz derecede şişireceği anlamına gelir. Bellekteki bu artış bir süre sonra her nekadar fiziki disk üzerinden karşılanabilecek olsada, performansı olumsuz etkiliyen ve sistem kaynaklarını tketen bir etkendir. Böyle bir durumda ön-bellekleme sisteminin avantajı kaybedilip dezavantaja dönüşebilir. Sonuç itibariye bir orta yolda hareket etmek çok daha mantıklı olabilir. Bu orta yol özellike web servislerinde yukarıdaki gibi parametreye bağımlı sonuç kümeleri söz konusu olduğunda ele alınan bir desen (pattern) içermektedir. Bu desene göre, parametreye bağlı sonuç kümelerinin çekildiği asıl veri seti ön-bellekte tutulur. Kullanıcının talepte bulunduğu veri içeriği ise, ön-bellekte tutulan bu ortak veri setinden ayrıştırılarak çekilir. Bu, ön-bellekleme kapasitesini daha etkin kullanabilmemizi sağlarken, kullanıcının isteklerini birazda olsa hızlı karşılayabilecek bir model oluşturabilmemizi sağlar. Aşağıdaki şekil bu modeli özetlemeye çalışmaktadır.

![mk169_4.gif](/assets/images/2006/mk169_4.gif)

Mavi çizgiler 5 numaralı ID değeri için gelen ilk talepte yapılan işlemleri göstermektedir. Tüm Products seti veritabanından çekilerek ön-belleğe atılır. Sonrasında ise ön-bellekteki veri seti içerisinden, ProductSubCategoryID değeri 5 olan satırlar ayrıştırılarak Client 1 isimli istemciye gönderilir. Aynı ProductSubCategoryID değeri için gelecek ikinci talepte ise artık ön-bellekte tutulmakta olan veri seti içerisinden ayrıştırma işlemi yapılacaktır. Database'e tekrar gidilip veri çekme işlemi gerçekleştirilmemektedir. Bu elbette veri setinin ön-bellekte tutulduğu süre içerisinde bir talep geldiğinde geçerlidir. Aksi durumda yine veritabanından ver çekme ve ön-belleğe alma işlemi gerçekleştirilecektir.

Sözü geçen modelde Data Caching sistemi etkin olarak kullanılmaktadır. Şimdi yukarıdaki örnek senaryomuzu bu modele göre yazmaya çalışalım. Herşeyden önce Product tablosunun içeriğini ilk gelen talepten sonra belirli bir süre ön bellekte tutacak bir metod yazmamız gerekiyor. Bu metod web servisi içerisindeki diğer metodlara hizmet vereceğinden private olarak tanımlanabilir. Metodun içerisinde, web servisindeki ilgili metoda (metodlara) gelen talepler için ön bellekte Product tablosuna ait bir DataSet nesnesinin tutulup tutulmadığına bakacağız. Bu kontrol işlemi için Cache sınıfından yararlanabiliriz. Eğer DataSet nesnemiz ön bellekte ise, web metoduna gelen parametre bağımlı talep buradan karşılanacak. Aksine ilgili veri seti ön bellekte değil ise, önce oluşturulacak ardından ön-belleğe atılacak ve son olarak yine ilgili web metoduna işlenmesi için devredilecek. Aşağıdaki metodumuz bu işlemler için tasarlanmıştır.

```csharp
private DataSet GetProducts()
{
    if (Context.Cache["Products"] != null)
    {
        return (DataSet)Context.Cache["Products"];
    }
    else
    {
        SqlDataAdapter da = new SqlDataAdapter("Select ProductID,Name,ListPrice,Size,StandardCost,ProductSubCategoryID From Production.Product", "data source=manchester;database=AdventureWorks;integrated security=SSPI");
        DataSet ds = new DataSet();
        da.Fill(ds);
        Context.Cache.Insert("Products", ds, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
        return ds;
    }
}
```

Burada Data Caching yaparken Absolute Time tekniğini seçtik. Yani Products isimli, DataSet taşıyan Cache nesnemiz oluşturulup ön belleğe atıldıktan sonra 5 dakika süreyle burada tutulacaktır. Elbette sahip olunan verinin içeriğinin ne kadar sık değiştiğine göre bu süre geliştirici tarafından uzatılabilir yada kısaltılabilir. Örneğin çok sık değişmeyen Ülke - Şehir - İlçe gibi bir içerik sunan verilerde Cache süresi mümkün olduğunca uzatılabilir.

Cache nesnesini bir web servisi metodu içerisinde kullanabilmek için güncel talepleri taşıyan http içeriğini (HttpContext) ele almamız gerekmektedir. Bu içeriği Context nesnesi yardımıyla ele alabiliriz. Products isimli, içeriğinde DataSet nesne örneği taşıyan bir Cache nesnesinin tutulup tutulmadığını kontrol etmek için ve eğer böyle bir nesne yok ise bunu oluşturdukan sonra ön-belleğe Cache sınıfı yardımıyla ekleyebilmek için Context sınıfından yararlanıyoruz. Gelelim istemcilere hizmete verecek olan parametre bağımlı metodumuza. Bu metodun içeriğinide aşağıdaki gibi tasarlayabiliriz.

```csharp
[WebMethod(Description = "Alt kategoriye göre ürünler")]
public DataSet GetProductsBySubCategory(int subCatId)
{
    DataSet dsResult = GetProducts().Copy();
    foreach (DataRow currRow in dsResult.Tables[0].Rows)
    {
        if ((currRow["ProductSubCategoryID"].ToString() == "") || (currRow["ProductSubCategoryID"] == null) || (Convert.ToInt32(currRow    ["ProductSubCategoryID"]) != subCatId))
        currRow.Delete();
    }
    dsResult.AcceptChanges();
    return dsResult;
}
```

Buradaki ana fikir ön bellekte tutulan veri seti içerisinden, kullanıcının talep ettiği parametre değerine göre oluşacak yeni bir veri setini kullanıcıya göndermektir. Bunun için, ön-bellekten gelen DataSet yapısını bozmayacak şekilde ele alınmaktadır. Bu sebeptende öncelikle ilgili DataSet'in bir kopyası Copy metodu ile çıkartılır. Sonrasında elde edilen kopya içerisinde dolaşılmakta ve ProductSubCategoryID değeri boş veya null olanlar ile gelen subCatId değerine eşit olmayanlar 0 indisli DataTable içerisinden çıkartılmaktadır. Böylece kullanıcıya istediği veri setini göndermiş oluruz. Yukarıdaki tekniği kullandığınız takdirde eğer web servisini çalıştırır ve GetProductsBySubCategory metodunu yürütürseniz, uygulamayı debug ederken metodun çalıştırılması sonrasında dönecek olan veri setinin sadece parametre olarak gelen kategori bilgilerini içerdiğini kolayca tespit edebilirsiniz.

![mk169_2.gif](/assets/images/2006/mk169_2.gif)

Peki bu işlemlerin bize kazandırdığı avantalar nelerdir? Her şeyden önce farklı parametre değerleri için ön bellekte farklı veri seti görüntüleri oluşturmaktan kurtulmuş oluyoruz. Bu sayede ön belleğin optimize edilişinde önemli bir kazancımız olmaktadır. Diğer taraftan tüm parametre bağımlı talepler ön bellekteki aynı veri setini kullandığından, veri ayrıştırma işlemi için veritabanına gidiş gelişler de azaltılmaktadır. Bu gidiş gelişlerin (round-trips) azalması, web servisinin sql sunucusunu çok fazla süre ile meşgul etmesinin önlenmesi anlamına gelmektedir. Bu makalemizde web servislerinde uygulanan ön bellekleme tekniğine farklı bir açıdan bakmaya çalıştık. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama İçin Tıklayın.](/assets/files/2006/AdventureServices.rar)

Burak Selim ŞENYURT
[selim (at) buraksenyurt.com](mailto:selim(at)buraksenyurt.com)