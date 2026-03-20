---
layout: post
title: "Asp.Net 2.0 ve Client Script Callback"
date: 2006-12-17 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - aspnet-2-0
  - csharp
  - dotnet
  - aspnet
  - http
  - javascript
  - caching
---
Web sayfalarımızda karşılaştığımız sorunlardan bir tanesi post-back hareketlerine neden olacak davranışların çok fazla olabilmesidir. Varsayılan olarak bir Button'ın görevi sayfayı istemciden sunucuya içeriği ile birlikte göndermektir. Diğer yandan bazı bileşenlerin AutoPostback özelliklerine true değeri atanarak istemci tarafından sunucuya doğru postalama işlemi yapmaları da sağlanabilmektedir. Fakat bazı durumlarda sayfanın tamamını sunucuya doğru postalamak istemeyebiliriz. Örneğin il ve ilçe bilgilerini ayrı ayrı tutan DropDownList kontrollerinin olduğu bir senaryoda; bir il seçildiğinde buna bağlı ilçelerin yüklenmesi için sayfanının tamamiyle sunucuya gidip gelmesi gerekecektir.

Böyle bir durumda sayfanın daha önceden istemci tarafına yüklenmiş pek çok parçası gereksiz yere tekrardan sunucu tarafından istemciye doğru gönderilecektir. Her ne kadar arabelleğe (caching) alma gibi tekniklerle sunucudan istemciye gelecek olan cevabın daha da hızlandırılmasını sağlayabilsekte, sayfanın tamamının postalanmasını engellemek ve sadece gereken kısımlarının güncellenmesi için ilgili parçaların postalanmasını sağlayabilmek daha etkili bir çözümdür. Tahmin edeceğiniz gibi bu yaklaşım modeli günümüzde Ajax ve Atlas.Net gibi teknolojik terimlerle karşımıza çıkmaktadır.

Biz bu makalemizde Asp.Net 2.0 üzerinde istemci tarafından geri bildirim (client script callback) işlemlerinin kolay bir şekilde nasıl yapılabileceğini incelemeye çalışacağız. İlk olarak çalışma modelinden bahsedelim. Başrol oyuncumuz ICallbackEventHandler arayüzüdür. Bu arayüzü ilgili web sayfasına ait sınıfamıza uygulamamız gerekmektedir. Uygulanan arayüz beraberinde iki metod sunmaktadır. Bu metodlar GetCallbackResult ve RaiseCallbackEvent üyeleridir. RaiseCallbackEvent metodu string tipinden bir parametre alır. Bu parametrenin rolü oldukça önemlidir. Nitekim istemci tarafından yada başka bir deyişle tarayıcı tarafından sunucuya doğru string tipte bir değer taşınması gerekmektedir. Örneğin illeri gösteren DropDownList kontrolünün seçilen değerinin string karşılığı, parametre olarak istemci tarafına gelebilir. GetCallbackResult metodu ise, geriye string tipinden bir değer döndürmektedir. Dolayısıyla bu değer istemci tarafından ele alınacak bilgiyi içermektedir. Örneğin seçilen ile bağlı ilçeleri bu dönüş değeri olarak düşünebiliriz.

Buradaki tek problem, istemciden sunucuya gelen yada sunucudan istemciye dönen bilgilerin string tabanlı olmasıdır. Bu sebepten istemci tarafındada bir Javascript kodunun GetCallbackResult metodundan dönen değeri ele alacak şekilde yazılmış olması gerekecektir. Benzer şekilde sunucudan istemciye gönderilecek string bilgininde özel olarak hazırlanması gerekecektir. Nitekim istemci tarafından ayrıştırılarak kullanılması söz konusu olabilir. Bu gereksinimlerin karşılanmasının ardından sunucu tarafında yapmamız gereken tek şey, istemcinin RaiseCallbackEvent metodunu nasıl tetikleyeceğini bildirmek olacaktır. Bu amaçlada.Net'in sunuduğu bazı metodlardan faydalanabiliriz. Bu metodlar yardımıyla sunucu tarafından istemci üzerine javascript kodu eklemek gibi işlemleri gerçekleştirebilmekteyiz. Sonuç olarak elde edeceğimiz mimari model aşağıdaki şekildekine benzer olacaktır.

![mk184_1.gif](/assets/images/2006/mk184_1.gif)

Dilerseniz bir örnek üzerinden devam edelim. Senaryomuzda AdventureWorks veritabanında yer alan ProductSubCategory ve ProductCategory tablolarını ele alacağız. İlk olarak web sayfamızı aşağıdakine benzer bir şekilde tasarlayarak işe koyulalım.

![mk184_2.gif](/assets/images/2006/mk184_2.gif)

İstemci Kategoriler'in tutulduğu DropDownList bileşeninden bir öğe seçtiğinde sayfanın tamamı sunucuya gönderilmeden, sadece bizim için gerekli olan Alt Kategoriler gelecek ve ilgili DropDownList bileşenine dolacak. Aslında Alt Kategorileri gösteren DropDownListe'e dolacağını söylemek çok doğru bir yaklaşım değildir. Nitekim, istemci tarafında var olan HTML çıktısında DropDownList kontrolü bir select takısı olarak görünür. Öğeleri ise içeride birer Option takısı olarak yer almaktadır.

![mk184_3.gif](/assets/images/2006/mk184_3.gif)

Dolayısıyla Alt Kategorileri doldurmak ile kastettiğimiz; istemciye gelen string cevabın içerisindeki alanları ayrıştırarak, ilgili select takısı içerisine birer option takısı olarak eklemektir. Tekrardan örneğimize dönebiliriz. İlk olarak kategorileri ProductCategories tablosundan yüklememiz gerekmektedir. Kategorileri doldurmak için SqlDataSource bilşeninden faydalanabiliriz. Select sorgumuz içerisinde ProductCategoryID ve Name alanlarını çekiyoruz.

```text
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Untitled Page</title>
    </head>
    <body>
        <form id="form1" runat="server">
            <div>Kategoriler<br />
                <asp:DropDownList ID="ddlKategoriler" runat="server" DataSourceID="dsKategoriler" DataTextField="Name" DataValueField="ProductCategoryID">
                </asp:DropDownList>
                <asp:SqlDataSource ID="dsKategoriler" runat="server" ConnectionString="<%$ ConnectionStrings:AdvConStr %>" SelectCommand="SELECT ProductCategoryID, Name FROM Production.ProductCategory">
                </asp:SqlDataSource>
                Alt Kategoriler<br />
                <asp:DropDownList ID="ddlAltKategoriler" runat="server">
                </asp:DropDownList>
            </div>
        </form>
    </body>
</html>
```

Bir sonraki adımımız olarak sayfamıza ICallbackEventHandler arayüzünü uygulamamız gerekiyor. Default.aspx sayfamız için sunucu tarafında yazılacak olan kodlar aşağıdaki gibi olacaktır.

```csharp
private string _kategoriId;

public string GetCallbackResult()
{
    using(SqlConnection conn=new SqlConnection(ConfigurationManager.ConnectionStrings["AdvConStr"].ConnectionString))
    {
        /* istemciden gelen CategoryId değerine göre SubCategory' ler çekilir ve bir StringBuilder yardımıyla bir dönüş bilgisi oluşturulur. */
        SqlCommand cmd = new SqlCommand("Select ProductSubCategoryID,Name From Production.ProductSubCategory Where ProductCategoryId=@CatId", conn);
        cmd.Parameters.AddWithValue("@CatId", _kategoriId);
        conn.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        StringBuilder builder = new StringBuilder();
        while (dr.Read())
        {
            builder.Append(dr["ProductSubCategoryID"].ToString());
            builder.Append("|"); // Field' ları birbirlerinden ayırabilmek için tek pipe konulur.
            builder.Append(dr["Name"].ToString());
            builder.Append("||"); // satır sonları için çift Pipe konulur.
        }
        dr.Close();
        return builder.ToString();
    }
}

public void RaiseCallbackEvent(string eventArgument)
{
    /* istemci tarafından tetiklenen bu metoda gelen string parametre değerini alıp sayfa seviyesindeki bir değişkene atıyoruz. */
    _kategoriId = eventArgument;
}
```

Burada GetCallbackResult metodundan geriye döndürdüğümüz string bilgiyi oluşturma şeklimiz eminimki dikkatinizi çekmiştir. Bunun en büyük nedeni az önce bahsettiğimiz gibi DropDownList kontrolünün istemci tarafında bir select takısına dönüşmesi ve elemanlarının birer option haline gelmesidir. Sonuç itibariyle istemci tarafındaki DropDownList kontrolünün Html çıktısının taşıyabileceği değerler göz önüne alındığında bunları toplu bir halde tek bir string olarak göndermek yeterli değildir.

Gönderilen string bilginin istemci tarafındaki fonksiyon içerisinde ayrıştırılabilmesi gerekecektir ki gelen bilgileri ayrıştırarak select takısı içerisine birer option olarak ekleyebilelim. Bu sebepten ProductSubCategoryId ve Name alanlarının arasına | gelecek ve her bir satırın sonundada || gelecek şekilde bir string oluşturulması tercih edilmiştir. Elbette burada | işaretini kullanmak zorunda değiliz. Bunun yerine yıldız yada virgül gibi sembolllerde konulabilir. Ancak elbette bu sembollerin okuduğumuz alanların içerisinde yer almamasına dikkat etmeliyiz. Böyle bir durumda istemci tarafındaki fonksiyon, satırları ve dolayısıyla alanları doğru bir şekilde ayrıştıramayacaktır.

Gelelim istemci tarafındaki fonksiyonumuza. Bu fonksiyonun temel görevi, GetCallbackResult metodundan dönecek olan string bilgiyi ayrıştırıp ddlAltKategoriler isimli dropdownList'in Html karşılığı olan içeriğine birer option elemanı olarak olarak eklemek olacaktır.

```text
<script type="text/javascript" language="javascript">

    function IstemciGeriBildirim(gelenBilgi,context)
    {
        /* Önce fonksiyona gelen altKategiler stringinin içeriğini taşıyacak DropDownList kontrolünü istemci tarafında yakalamılıyız. */
        var lstKategoriler=document.forms[0].elements['ddlAltKategoriler'];
        lstKategoriler.innerHTML=""; // liste içeriği temizlenir
        // Gelen string bilgiyi || lara göre ayrıştıyoruz.
        var satirlar=gelenBilgi.split('||');

        // her bir satırı dolaşıyoruz.
        for(var i=0;i<satirlar.length;++i)
        {
            // satırları | işaretine göre ayrıştırıyoruz. Böylece alanları elde ediyoruz.
            var alanlar=satirlar[i].split('|');
        
            var altKategoriId=alanlar[0];
            var altKategoriAdi=alanlar[1];

            // Html tarafında listemiz için option elementini oluşturuyoruz. Value olarak altKategoriId' yi içerik olarakta altKategoriAdi' ni veriyoruz.
            var oge=document.createElement('option',altKategoriId);
            oge.innerHTML=altKategoriAdi;

            // Oluşturulan öğe listeye eklenir.
            lstKategoriler.appendChild(oge);
        }
    } 

</script>
```

Bu JavaScript fonksiyonu sunucudaki GetCallbackResult metodundan dönen string bilgiyi ayrıştırmak ve istemci tarafındaki liste kutusuna eklemekten sorumludur. Unutulmamalıdır ki, istemci tarafına gelen bilgiler HTML içeriğidir. Dolayısıla DropDownList'e eleman eklemek için ayrıştırılan her bir öğenin birer option elementi olarak ele alınması gerekmektedir. Yapmamız gereken son bir işlem daha vardır. İstemci tarafında, Kategorilerin tutulduğu liste kutusu üzerinde bir bir öğeden bir diğerine geçildiğinde, sunucu tarafındaki ilgili metodların tetiklenmesi gerekmektedir. Bunu gerçekleştirebilmek için sayfanın PageLoad olay metodu içerisine aşağıdaki kod satırlarını eklememiz yeterli olacaktır.

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    string script=ClientScript.GetCallbackEventReference(this, "document.all['ddlKategoriler'].value", "IstemciGeriBildirim", null);
    ddlKategoriler.Attributes.Add("onChange", script);
}
```

GetCallbackEventReference isimli metod sayesinde istemci tarafı için gerekli olan script'lerin otomatik olarak oluşturulmasını sağlayabiliriz. Metoun ilk parametresi, client callback işleminin ele alınacağı nesneyi belirtmektedir. Uygulamamız göz önüne alındığında bu nesne sayfanın kendi referansıdır. İkinci parametre istemci tarafından sunucu tarafındaki RaiseCallbackEvent metoduna gelecek olan parametredir ki bu senaryoda ddlKategoriler'de seçili olan öğenin değeridir. Üçüncü parametre ile istemci tarafında, sunucudaki GetCallbackResult metodunun sonucunu ele alacak olan JavaScript fonksiyonunun adı belirtilir.

Son parametre istemci tarafındaki fonksiyona geçirlmesi gereken ekstra bilgiler var ise kullanılır. Örneğimizde böyle bir ihtiyacımız olmadığı için son parametre değerini null olarak geçiyoruz. Son olarak, scriptlerin hangi kontrolün hangi niteliği için oluşturulacağını belirtiyoruz. Örneğimizde liste kutusunun onChange olayı için gerekli JavaScript'lerin oluşturulmasını belirtmekteyiz. (Aşağıdaki görüntüyü izleyebilmek için en azından Flash 6.0 sürümü gerekmektedir.)

Gördüğünüz gibi Kategorilerde değişiklik yaptığımızda alt kategoriler listesi dolmakta, ancak sayfanın sunucuyu tamamiyle postalandığına dair bir işaret görülmemektedir.

Client Script Callback mimarisi derinlemesine incelendiğinde sayfanın istemciden sunucuya doğru tamamen postalanmayışı, sayfaya ait bazı olay metodların çalışmadığı anlamına gelmez. Aslında sayfaya ait Page_Init, Page_Load ve Page_Unload gibi olaylar çalışmaktadır. Bununla birlikte örneğin PreRender, Render gibi olaylar ve hatta post-back olaylarına ait metodlar çalıştırılmamaktadır. Dolayısıyla ilgili sayfa için, bir bölümü kırpılmış bir yaşam döngüsünün söz konusu olduğunu ve çalıştığını söyleyebiliriz.

Client Script Callback ile ilgili dikkat edilmesi gereken bazı kısıtlamalarda vardır. Günümüzde hemen her tarayıcı programın JavaScript desteği vardır. Ancak bazı tarayıcıların Client Callback desteği olmayabilir. Hatta internet explorer bu sistemi gerçekleştirmek için ActiveX kullanır ve tarayıcının ActiveX ayarları güvenlik nedeniyle bilerek kapatılmış olabilir. Bu gibi sebeplerden dolayı Client Callback mimarisi doğru bir şekilde çalışmayacaktır. İstemcilerin kullandıkları tarayıcı programların ClientCallback desteğinin olup olmadığını öğrenmek ve uygulamanın işleyişini buna göre değiştirmek için Request.Browser.SupportsCallback özelliğini kullanabiliriz. Bu makalemizde kısaca istemci taraflı callback modelinin,.Net üzerinde nasıl gerçekleştirilebileceğini incelemeye çalıştık. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın.](/assets/files/2006/ClientCallback.rar)