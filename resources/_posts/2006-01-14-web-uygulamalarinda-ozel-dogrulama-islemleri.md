---
layout: post
title: "Web Uygulamalarında Özel Doğrulama İşlemleri"
date: 2006-01-14 10:00:00 +0300
categories:
  - aspnet
tags:
  - aspnet
  - csharp
  - javascript
  - dotnet
  - authentication
  - generics
---
Web uygulamalarında, kullanıcıların girmiş olduğu verilerin istenen şartlara göre doğruluklarının kontrol edilmesi son derece önemlidir. Asp.Net ile geliştirilen web uygulamalarında, kullanıcı girişlerinin kontrolü için çoğunlukla validation kontrolleri kullanılır. Validation bileşenleri hem istemci tarafında hemde sunucu tarafından veri kontrol işlemlerini gerçekleştirebilir. (Bu makaleyi daha kolay takip edebilmeniz açısından var olan Validator kontrollerinin kullanımını bildiğiniz varsayılmaktadır.)

> Temel olarak bir verinin doğruluğunun Validator bileşenleri ile kontrol işlemi, eğer istemci script çalıştırılmasına izin veriyorsa, önce istemci tarafında client script'ler yolu ile gerçekleştirilir. İstemci tarafında kontrol yapılsada, yapılmasada mutlaka ve mutlaka server tarafında da bir doğrulama işlemi gerçekleştirilmektedir.

Temel doğrulama işlemlerinde, girilen verinin belli bir formata uygun olup olmadığı (örneğin geçerli bir mail adresi olup olmadığı), boş olup olmadığı, herhangibir değerden küçük, büyük, eşit vb... olup olmadığı ve benzeri durumlar kontrol edilir. Gerçek şudurki, var olan Validation kontrolleri ile hemen hemen her tür doğrulama işlemini gerçekleştirebiliriz. Ancak bazı durumlarda veri üzerinde doğrulama işlemleri için özel algoritmalara ihtiyacımı olabilir.

Örneğin, girilen verinin uygun bir kredi kartı numarası olup olmadığının kontrol edilmesini sağlayan bir algoritmayı, bir validation kontolü içerisinde kullanmak isteyebiliriz. Yada birden fazla doğrulama işlemini bir arada sunmak isteyebiliriz. İşte bu ve benzer durumlar için Asp.Net, CustomValidator isimli bir bileşen içermektedir. Bu bileşen yardımıyla sunucu ve istemci tarafında çalışacak özel kontrol algoritmalarımızı veya süreçlerimizi, çeşitli veri giriş kontolleri ile ilişkili olacak şekilde yazabiliriz. Bu makalemizde, basit olarak Lhun algoritması yardımıyla kredi kartı doğrulama kriterini uygulayan bir CustomValidator örneği geliştireceğiz. Uygulamamız sadece Lhun Algoritma kontrolünü yapacak.

CustomValidator kontrolümüzü kullanmaya başlamadan önce, kısaca Lhun algoritması hakkında da bilgi vermekte fayda olacağı kanısındayım. Lhun algoritması basit olarak kredi kartı gibi sayısal ifadelerin doğruluğunu kontrol etmek amacıyla kullanılan bir matematik algoritmasıdır. Bu algoritmaya "mod 10 algoritması" da denmektedir. Basit olarak bir dizi matematiksel işlem ile, verinin uygun bir kredi kartı numarası olup olmadığı tespit edilir. Eğer kredi kartı numarası uygun ise, sıradaki diğer işlemlere geçilebilir.

> Lhun algoritması sadece girilen sayısal ifadenin uygun bir kredi kartı numarası olup olmadığını belirten bir model sunar. İstemci tarafından girilen kredi kartı numarasının doğru olması sadece numaranın dünya çapında kabul görmüş bir algoritma ile doğrulanabildiğini gösterir. Oysaki, girilen kredi kartı numarasının geçerlilik süresinin, kart sahibinin isminin ve CVV2 gibi diğer kriterlerin kontrolüde gerekir ki bu tamamen ayrı bir süreçtir.

Kısaca Lhun algoritması şu şekilde çalışır.

1 - İlk olarak kredi kartı numarasının en sağ ikinci dijitinden başlanarak sırasıyla sola doğru ilerlenir. (Kod yazılırken bunu göz ardı edip soldan ikinci dijitten başlayıp ikişer ikişer de atlayabiliriz.) İkişer ikişer atlanırken her bir dijitin iki katı hesap edilir. Elde edilen sonuçlardan değeri 10 ve 10' dan büyük olanlar var ise bunların basamakları toplanır ve diğer 10' dan küçük olan değerler eklenerek bir toplam değeri elde edilir.

2 - Daha sonra, iki katı alınan dijitlerin dışında kalan dijitler ele alınır ve bu dijitler bir birleriyle toplanarak bir toplam değeri daha elde edilir.

3 - Son olarak 1nci ve 2nci işlemlerdeki toplamların toplamı alınır ve sonucun 10 ile bölünüp bölünmediğine (bir başka deyişle mod 10' un sıfır olup olmadığına) bakılır. Eğer 10 ile tam bölünebiliyorsa bu sayı dizisi bir kredi kartı numarasıdır. Olayı daha iyi anlamak için örnek bir 16 haneli sayı dizisini ele alalım.

![mk144_1.gif](/assets/images/2006/mk144_1.gif)

Burada 1234 5678 9876 5432 sayı dizisinin geçerli bir kredi kartı numarası olup olmadığının Lhun algoritmasına göre nasıl tespit edilebildiğini görmektesiniz. Dikkat ederseniz sonuç 10 ile tam olarak bölünemediğinden sayı dizisi geçerli bir kredi kartı numarasını temsil etmemektedir. Sahip olduğunuz kredi kartları üzerinde yukarıdaki algoritmayı deneyebilir ve sonuçlarını irdeleyebilirsiniz. Şunuda belirtmekte fayda vardır ki, dünya çapında kullanılan çeşitli tipte kredi kartları mevcuttur. Örneğin master card, visa gibi. Bunlarında kendilerine has bir takım sayı dizisi kuralları vardır. Örneğin bir master card'a ait kredi kartı numarasının ilk iki hanesi, 55 yada 50 olmak zorundadır. Bu, kartın bir master card olduğunun işaretidir. Konumuz validation işlemlerinin CustomValidator kontrolü ile nasıl gerçekleştirilebileceğini incelemek olduğundan, Lhun algoritması üzerinde daha fazla durmayacağız.

Gelelim bu algoritmayı kullanacağımız uygulama kodlarımıza. CustomValidator kontrolümüz sıradan bir Validator kontrolünden farksızdır. Sadece kontrol işleminin yapılacağı olay metodlarını hem client (istemci) tarafında hem de sunucu (server) tarafında kendimiz yazmamız gerekmektedir. Elbette istemci tarafında bir kontrol kodu yazmak zorunda değiliz. Ancak sunucu tarafında mutlaka yazmalıyız. Aksi takdirde doğrulama işlemlerini gerçekleştiremeyiz. Server tarafında yazılan kodlar, ServerValidate olay metodunda ele alınır. Client tarafında yazılacak olan script kodların yer alacağı fonksiyon ise, ClientValidationFunciton özelliğinde belirtiriz.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Normalde, diğer Validator kontrolleri, client script özellikleri kapatılmadığı takdirde, istemci tarafında çalışacak script kodlarını otomatik olarak üretmektedir. Ancak CustomValidator kontrolü söz konusu olduğunda, istemci tarafında çalışacak script kodlarınıda manuel olarak yazmamız gerekmektedir.

ServerValidate metodunun ServerValidateEventArgs parametresi, kontrol edilecek bileşene ait özelliğin değerini temsil eder. Örneğin kredi kartı numarasının girileceği TextBox bileşeninin, Text özelliğinin değerini bu parametrenin Value özelliği ile metod içerisinde alabiliriz. Bu parametrik yapı client tarafında çalışacak script metodu içinde geçerlidir. Bizim örnek kodumuzda yapacağımız kontroller sırasıyla, girilen sayı dizisinin 16 haneli olduğu, sadece sayılardan oluştuğu ve Lhun algoritmasını sağlayıp sağlamadığıdır. Default.aspx sayfamızın tasarımı aşağıdaki gibi olacaktır. (Örneğimiz Asp.Net 2.0 platformunda geliştirilmiştir.)

![mk144_2.gif](/assets/images/2006/mk144_2.gif)

CustomValidator bileşenin ControlToValidate özelliği, TextBox bileşenine ayarlanmıştır. Nitekim doğrulama işlemi için TextBox kontrolümüz ele alınacaktır. Son olarak, Kontrol başlıklı butonumuz sadece sayfayı postback etmek ve bu sayede server tarafındaki doğrulama sürecine geçebilmek amacıyla kullanılmaktadır. Yani herhangibir kod içermemektedir. Sadece postback işlemini sağlar. İlk olarak ServerValidate olay metodunu aşağıdaki gibi kodlayalım.

```csharp
protected void custVldtr_ServerValidate(object source, ServerValidateEventArgs args)
{
    #region Hane Sayısı Kontrolü

    if (args.Value.Length != 16)
    {
        // Hata mesajı değiştirilir.
        custVldtr.ErrorMessage = "Kart numarası 16 haneli olmalıdır.";
        args.IsValid = false; // Validation işlemi geçersizdir.
        return; // Metoddan çıkılır.
    }

    #endregion

    #region Sayısal değer kontrolü

    for (int i = 1; i < args.Value.Length; i++)
    {
        if (!char.IsDigit(args.Value[i]))
        {
            // Hata mesajı değiştirilir.
            custVldtr.ErrorMessage = "Sadece sayısal değer girilmelidir.";
            args.IsValid = false; // Validation işlemi geçersizdir.
            return; // Metoddan çıkılır.
        }
    }

    #endregion

    #region Lhun Kontrolü

    List<int> kartNumarasi = new List<int>();
    List<int> ciftKartNumaralari = new List<int>();
    int toplam1=0,toplam2 = 0;

    // TextBox' a girilen string formattaki kart numarasina ait sayı dizisinin her bir elemanı List tipinde int' değerler tutan generic kartNumarasi isimli koleksiyona aktarılır.
    for (int i =0;i<args.Value.Length;i++)
    {
        kartNumarasi.Add(Convert.ToInt16(args.Value[i].ToString()));
    } 

    // ilk olarak iki katı hesaplaması ve çıkan sayıların toplamı işlemi yapılır.
    for (int i =0; i <kartNumarasi.Count; i =i+ 2)
    {
        ciftKartNumaralari.Add(kartNumarasi[i] * 2);
    }

    for (int i = 0; i < ciftKartNumaralari.Count; i++)
    {
        if (ciftKartNumaralari[i] > 9)
        {
            string var = ciftKartNumaralari[i].ToString();
            toplam1 += Convert.ToInt16(var[0].ToString()) + Convert.ToInt16(var[1].ToString());
        }
        else
        {
            toplam1 += ciftKartNumaralari[i];
        }
    }

    // iki katı hesabı dışında kalan elemanların toplamı hesaplanır.
    for (int i = 1; i < kartNumarasi.Count; i += 2)
    {
        toplam2 += kartNumarasi[i];
    }

    // Genel toplam alınır ve 10 ile tam bölünüp bölünmediğine bakılır.
    int toplam = toplam1 + toplam2;
    if (toplam % 10 == 0)
    {
        args.IsValid = true; // hata mesajı döndürülmez. Validation işlemi geçerlidir.
    }
    else
    {
        // Hata mesajı değiştirilir.
        custVldtr.ErrorMessage = "Geçersiz kredi kartı numarası girdiniz.";
        args.IsValid = false; // Validation işlemi geçersizdir.
    }

    #endregion
}
```

ServerValidate metodu geri dönüş değeri olmayan bir metoddur. ServerValidateEventArgs parametresi sayesinde, ControlToValidate özelliği ile bağlanan kontrolün (ki burada TextBox kontrolüdür) doğrulama sürecine girecek değerine erişilmektedir. Bu parametrenin IsValid özelliği bool tipinden bir özelliktir ve doğrulama işleminin doğruluğunu belirtmektedir. True değerini aldığı takdirde doğrulama geçerlidir. False değerinde ise doğrulama işlemi geçersizdir. Doğrulama işlemi geçersiz olduğu takdirde uygulama işleyişini durduracak ve hata mesajı ile kullanıcı bilgilendirilecektir. Bu sayede izleyen süreçlerinde tutarlılığını sağlamış oluruz. ServerValidate metodumuz Server (sunucu) tarafında çalışmaktadır. Uygulamamızı bu haliyle çalıştırdığımızda aşağıdaki Flash animasyonunda görülen sonuçları elde ederiz.

(Not: Aşağıdaki görüntüyü seyredebilmek için tarayıcınızda Flash Player'ın son sürümünün olması tavsiye edilir. Eğer sisteminizde XP Service Pack 2 yüklüyse ilgili uyarıyı dikkate alıp içeriğe izin vermelisiniz. (Allow Blocked Content). Videoyu yönetmek için sağ tıklayıp çıkan menüyü kullanabilirsiniz.)

Dikkat ederseniz, sayfa sunucuya geri gönderildikten sonra doğrulama işlemi devreye girmektedir. Şu anda client (istemci) tarafı için bir script kodu yazmadığımızdan, normal doğrulama sürecine ait olan istemci kontrolü kısmı otomatik olarak devre dışıdır. Oysaki çoğu zaman sunucuya geri dönülmeden istemci tarafında kontroller yapmak isteyebiliriz. Ancak.Net doğrulama sistemi göz önüne alındığında istemci tarafında herşey doğru olsa bile, sunucu tarafında yinede doğrulama işlemi gerçekleştirilmektedir. O halde client (istemci) tarafında yapılan doğrulamanın ne gibi bir avantajı olabilir? İstemci tarafında yapılan doğrulama ile, sunucuya gereksiz yere gidip gelme işleminin önüne geçmiş oluruz.

![dikkat.gif](/assets/images/2006/dikkat.gif)
Hatırlatma; Doğrulama süreci şu şekilde işler.
![mk144_3.gif](/assets/images/2006/mk144_3.gif)

Şimdi CustomValidator kontrolümüz için bir de client (istemci) script kodunu ekleyelim. Bu amaçla javascript kullanmayı seçtiğimizi düşünecek olursak tek yapmamız gereken aspx sayfamıza aşağıdaki script kodlarını eklemek olacaktır.

```javascript
<script type="text/javascript">
function ValidateCreditCard(sender,args)
{
    if (args.Value.length != 16)
    {
        alert("Kart numarası 16 haneli olmalıdır.");
        args.IsValid = false;
        return;
    }

    var rakamlar = '0123456789';

    for (i=0; i<args.Value.length; i++) 
    {
        if (rakamlar.indexOf(args.Value.charAt(i),0) == -1) 
        {
            alert("Sadece sayısal değerler girilebilir.");
            args.IsValid=false;
            return;
        }
    }

    var toplam=0;
    for (i=0; i < args.Value.length; i++) 
    {
        var numara=args.Value.charAt(i);
        if (i % 2 == 0) 
            numara=numara * 2;
        if (numara > 9) numara=numara - 9;
            toplam = toplam + parseInt(numara);
    }
    if(toplam % 10!=0)
    {
        alert("Geçersiz kredi kartı.");
        args.IsValid=false;
        return;
    } 
}
</script>
```

CustomValidator kontrolümüzün aspx tarafındaki kodları ise aşağıdaki gibidir.

```text
<asp:CustomValidator ID="custVldtr" runat="server" ControlToValidate="txtCreditCardNumber" OnServerValidate=custVldtr_ServerValidate" ClientValidationFunction="ValidateCreditCard"> </asp:CustomValidator>
```

İstemci tarafında çalışan uygulamamızı aşağıdaki flash animasyonunda daha kolay izleyebilirsiniz.

(Not: Aşağıdaki görüntüyü seyredebilmek için tarayıcınızda Flash Player'ın son sürümünün olması tavsiye edilir. Eğer sisteminizde XP Service Pack 2 yüklüyse ilgili uyarıyı dikkate alıp içeriğe izin vermelisiniz. (Allow Blocked Content). Videoyu yönetmek için sağ tıklayıp çıkan menüyü kullanabilirsiniz.)

İstemcilerin doğası gereği, her zaman client tarafında çalışacak script'lere izin verilmez. İstemci tarafında çalışan doğrulama script'leri, sunucuya doğru yapılan gidiş-geliş işlemlerinin sayısını azaltan bir etkendir; ki buda ağ üzerindeki gereksiz yükü azaltır. Ancak bazı istemcilerin, script çalıştırma izni olmadığını için, sunucu tarafında doğrulama işlemi yapılması gerekliliği söz konusudur. Biz örneğimizde CustomValidator kontrolü için hem istemci tarafında hem de server tarafında doğrulama işlemini uygulayabileceğimiz iki metod kullandık. Böylece istemci izin veriyorsa doğrulamayaı o tarafta yapıp gereksiz ağ yükünü azaltmış olduk. Özel algoritmalar içeren veya birden fazla doğrulama işlemini bir kontrol üzerinde birleştirmek istediğimiz durumlarda CustomValidator kontrolü oldukça işimize yaramaktadır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.