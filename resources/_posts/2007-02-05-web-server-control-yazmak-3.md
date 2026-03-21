---
layout: post
title: "Web Server Control Yazmak - 3"
date: 2007-02-05 12:00:00 +0300
categories:
  - aspnet-2-0
tags:
  - asp.net
  - web-server-controls
---
Bir önceki makalemizde kendi web kontrollerimizi geliştirirken durum yönetimi (state management) için ViewState'lerden nasıl faydalanabileceğimizi incelemiştik. Bununla birlikte assembly, sınıf ve metod seviyesinde, çalışma zamanı için gerekli davranışları çeşitli nitelikler (attribute) yardımıyla nasıl etkileyebileceğimizi görmüştük. Bugünkü makalemizde ise, kendi web kontrollerimiz için olay güdümlü (event based) programlamayı nasıl sağlayabileceğimizi incelemeye çalışacağız.

Var olan web kontrollerinin pek çok olayı vardır. İstersek kendi geliştirdiğimiz web sunucu kontrolleri içinde olay tanımlayabilir ve kullanabiliriz. Ne varki web kontrolleri seviyesinde dikkate alınması gereken bazı noktalar vardır. Bu noktaları yazımızın ilerleyen kısımlarında keşfedeceğiz. Öncelikli olarak olay tabanlı programlama için gereken unsurları biraz hatırlamaya çalışalım. Herşeyden önce olayların (events), temsilciler (delegates) ile yakın ilişkisi vardır. Olay güdümlü programlama tarafında temsilcilerin görevi, meydana gelen olay sonrasında çalıştırılacak olan olay metodunu işaret etmektir. Var olan Windows ve Web kontrollerinin sahip oldukları olaylar, yine Framework içerisinde yer alan değişik temsilciler ile ilişkilendirilmiştir. Örneğin bir Button bileşenine ait click olayı, EventHandler temsilcisi ile ilişkilendirilmiştir. Temsilciler çalışma zamanında işaret edecekleri metodların yapılarınıda söylemektedirler. Bir başka deyişle işaret edilecek metodun parametrik yapısı ve geri dönüş tipinide belirtirler. Özellikle görsel bileşenlere bakıldığında, olay metodlarının çoğunlukla iki parametre aldığını görürüz. Bu parametrelerden ilki, olayı meydana getiren nesne referansını taşıyabilecek object tipindendir. İkinci parametre ise, olay meydana geldiğinde, olay metodu içerisine bilgi taşımak amacıyla kullanılabilecek türden bir tip olabilir.

Bu kriterler göz önüne alındığında, kendi web kontrollerimizi geliştirirken Framework içerisinde var olan temsilcilerden faydalanabiliriz. Ancak özellikle olay metodunun ikinci parametresinin, olay metodu içerisine bizim istediğimiz tarzda bilgiler taşımasını istiyorsak, kendi temsilcilerimizi yazmayı tercih edebiliriz. Biz makalemizde kendi temsilci tipimizi kullanacağız. Örneğin TarihKontrolum isimli bileşenimiz için TarihDogrula isimli bir olay düşünebiliriz. Bu olay meydana geldiği zaman çalışacak olan olay metodu ikinci parametre olarak, kontrol içerisindeki verilerin değerlerini alabilir ve içeride işleyebilir. Bu senaryo göz önüne alındığında, öncellikli olarak olay metoduna gelecek bilgileri taşıyacak bir parametre sınıfı yazmamız muhakkaktır. Bu sınıfı aşağıdaki gibi geliştirdiğimizi düşünelim.

![mk190_1.gif](/assets/images/2007/mk190_1.gif)

```csharp
public class TarihKontrolumEventDatas
{
    #region Fields

    private string seciliGun;
    private string seciliAy;
    private string seciliYil;

    #endregion

    #region Properties

    public string SeciliYil
    {
        get { return seciliYil; }
    }

    public string SeciliAy
    {
        get { return seciliAy; }
    }

    public string SeciliGun
    {
        get { return seciliGun; }
    }

    #endregion

    #region Constructors

    public TarihKontrolumEventDatas(string sg, string sa, string sy)
    {
        seciliGun = sg;
        seciliAy = sa;
        seciliYil = sy;
    }

    #endregion
}
```

TarihKontrolumEventDatas isimli sınıfın temel görevi, olay metodu içerisine, kontrolün içerisindeki gün, ay ve yıl değerlerini taşımaktır. Buna göre temsilcimizide (delegate) aşağıdaki gibi tanımlayabiliriz.

![mk190_2.gif](/assets/images/2007/mk190_2.gif)

```csharp
public delegate void TarihDogrulaEventHandler(object sender,TarihKontrolumEventDatas eData);
```

Temsilcimiz object ve TarihKontroumEventDatas tipinden parametreler alan ve geriye değer döndürmeyen metodları işaret edebilecek şekilde tasarlanmıştır. Bu tipik olarak varsayılan olay temsilcisi desenidir. Artık kontrolümüz için gerekli olan olayı (event) tanımlayabilir ve bu olayı tetikletecek üye metodu ekleyebiliriz. Bu amaçla TarihKontrolumWithEvent isimli yeni kontrolümüze aşağıdaki eklemeleri yapmamız gerekmektedir.

```csharp
[Description("Tarih bilgisi değiştirildiğinde doğrulamak için tetiklenen sunucu taraflı olaydır")]
public event TarihDogrulaEventHandler TarihDogrula;

public void OnTarihDogrula(TarihKontrolumEventDatas eData)
{
    if (TarihDogrula != null)
        TarihDogrula(this, eData);
}
```

OnTarihDogrula isimli metodun tek görevi, kontrole yüklenmiş TarihDogrula isimli bir olay var ise bu olayı çağırmaktır. Bu çağrıda çok doğal olarak, olay ile ilişkilendirilmiş metodun çağırılması anlamına gelmektedir. Bu değişikliklerden sonra kontrolümüzü herhangibir Asp.Net sayfasına sürükleyip bıraktığımızda, events kısmında yeni eklediğimiz olayı görebiliriz.

![mk190_3.gif](/assets/images/2007/mk190_3.gif)

Her ne kadar kontrolümüze bir olay eklemiş olsakta, bu olayın gerçekleştirilmesi için sayfanın istemciden sunucuya tekrardan gönderilmesi gerekecektir. Bu işi kontrolümüz üzerinde gerçekleştirmek istediğimizi düşünecek olursak bize submit eyleminde bulunacak bir input Html kontrolü gerekmektedir. Bu sebepten yeni kontrolümüzün Render metodu içerisine aşağıdaki Html içeriğini dahil etmemiz uygun olacaktır.

```csharp
protected override void Render(HtmlTextWriter writer)
{ 
    // Diğer kod satırları
    writer.Write("<input type='submit' value='...' name='sbmButton'/>");
    base.Render(writer);
}
```

Artık web sayfamızda yer alan kontrolümüz için TarihKontrol olayını aşağıdaki gibi yükleyebiliriz.

![mk190_4.gif](/assets/images/2007/mk190_4.gif)

```csharp
protected void TarihKontrolumWithEvent1_TarihDogrula(object sender, BenimWebKontrollerim.TarihKontrolumEventDatas eData)
{
    Response.Write(eData.SeciliGun + " " + eData.SeciliAy + " " + eData.SeciliYil);
}
```

Şimdi bu olay metodu içerisine bir breakpoint koyalım ve web uygulamamızı debug mode üzerinden çalıştıralım. Kontrolümüz içerisinde yer alan submit düğmesine bassakta, olay metodumuz içerisine giremediğimizi göreceğiz. Dolayısıyla yazdığımız olay metodu hiç bir şekilde işlememiştir. Bunun iki önemli nedeni vardır. Bunlardan birincisi, kontrolümüz içerisinde postback işlemini gerçekleştiren düğmenin adının kontrolümüzün benzersiz adı (unique Id) ile aynı olmamasıdır.

İkinci sebep ise, istemciden postback ile gelecek olan form verileri içerisinden ay, gün ve yil alanlarının istemci tarafından değiştirilen değerlerini elde etmek ve eğer bir değişiklik varsa ilgili olay metodunu fırlatmak için gerekli hamleyi yapmayışımızdır. Bahsetmiş olduğumuz ikinci hamle için, kontrolümüze IPostBackDataHandler arayüzünün uyarlanması (implementation) düşünülebilir.

> Kendi yazdığımız web kontrollerinde event, bu event ile ilişkili bir delegate, event metodu içerisine bilgi taşıyacak bir tip tanımlamak event based bir kontrol için yeterli değildir. Kontroldeki bir aksiyon sonucu, istemcideki sayfayı sunucuya gönderecek olan bileşenin adının kontrolün adı ile aynı olması (UniqueId değeri), kontrolün post edilen verilerinin (postBack Datas) alıp işlenmesi ve ilgili olay metodunun fırlatabilmesi içinIPostBackDataHandler arayüzünün uygulanmış olması gerekmektedir. Eğer sadece postback olaylarını fırlatmayı düşünüyorsak, yani form verilerini işlemeyi düşünmeden olay fırlatmak ile ilgileniyorsak IPostBackEventHandler arayüzünü uygulamamız yeterli olacaktır.

İlk yapmamız gereken değişiklik, Render metodu içerisinde aşağıdaki gibi olmalıdır. Bu amaçla kontrolümüz içerisine birde özellik (property) katabiliriz. Bu özellik yanlız okunabilir (ReadOnly) olarak tasarlanabilir ve görev olarak, o an üretilen bileşenin UniqueId değerini taşıyabilir. Aynen aşağıdaki gibi.

```csharp
public string SubmitName
{
    get { return this.UniqueID; }
}

protected override void Render(HtmlTextWriter writer)
{ 
    // Diğer Kodlar
    writer.Write("<input type='submit' value='...' name='" + SubmitName + "'/>");
    base.Render(writer);
}
```

Şimdide kontrolümüze IPostBackDataHandler arayüzünü uygulamalıyız. Bu arayüz içerisinde iki önemli metod vardır. LoadPostData ve RaisePostDataChangedEvent. Özellikle LoadPostData metodu sayesinde, istemciden sunucuya gönderilen sayfa içerisindeki form verisini çekebilir ve kontrolümüzün içerisindeki bazı alanların değerlerini yakalayabiliriz ki buda istediğimiz bir durumdur. (Hatırlayınız, postback nedeni ile kontrol üzerinde yapılan değişiklikleri kaybediyorduk.) LoadPostData metodunun çalışması sonrasında eğer true değeri üretilirse, RaisePostDataChanged metodu devreye girecektir.

Aksi takdirde RaisePostDataChangedEvent metodu çalıştırılmayacaktır. Buda ilgili olay metodun, kontrol üzerindeki form verilerinde değişiklik olduğu zaman tetiklenmesine neden olacaktır ki bu bir avantaj olarak düşünülebilir. Ayrıca RaisePostDataChangedEvent metodu, postback sonrası kontrole yüklenmiş olan olayı çağırmamızda da işe yarayacaktır. Buna göre, kontrolümüzün bu arayüzü uygulamış olan son hali aşağıdaki gibi olacaktır.

![mk190_5.gif](/assets/images/2007/mk190_5.gif)

```csharp
public class TarihKontrolumWithEvent : Control,IPostBackDataHandler
{
    [Description("Tarih bilgisi değiştirildiğinde doğrulamak için tetiklenen sunucu taraflı olaydır")]
    public event TarihDogrulaEventHandler TarihDogrula;

    public void OnTarihDogrula(TarihKontrolumEventDatas eData)
    {
        if (TarihDogrula != null)
            TarihDogrula(this, eData);
    }

    // GunMetin, AyMetin, YilMetin,SeciliGun,SeciliAy,SeciliYil özellikeri kodun kolay okunması amacıyla buradan kaldırılmıştır. Örnek uygulamayı indirip bu kodları da tedarik edebilirsiniz.

    public string SubmitName
    {
        get { return this.UniqueID; }
    }
    #endregion

    protected override void Render(HtmlTextWriter writer)
    { 
        writer.Write("<span id='lblGun'>" + GunMetin + "</span>");
        writer.Write("  ");
        writer.Write("<select name='"+this.UniqueID+"Gun' id='Gun'>");
        for (int i = 1; i <= 31; i++)
        {
            if (SeciliGun == i.ToString())
                writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            else
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        }
        writer.Write("</select>");
        writer.Write("  ");
        writer.Write("<span id='lblAy'>" + AyMetin + "</span>");
        writer.Write("  ");
        writer.Write("<select name='"+this.UniqueID+"Ay' id='Ay'>");
        for (int i = 1; i <= 12; i++)
        {
            if (SeciliAy == i.ToString())
                writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            else
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        }
        writer.Write("</select>");
        writer.Write("  ");
        writer.Write("<span id='lblYil'>" + YilMetin + "</span>");
        writer.Write("  ");
        writer.Write("<select name='"+this.UniqueID+"Yil' id='Yil'>");
        for (int i = 1950; i <= 2050; i++)
        {
            if (SeciliYil == i.ToString())
                writer.Write("<option selected='selected' value='" + i.ToString() + "'>" + i.ToString() + "</option>");
            else
                writer.Write("<option value='" + i.ToString() + "'>" + i.ToString() + "</option>");
        }
        writer.Write("</select>");
        writer.Write("<input type='submit' value='...' name='" + SubmitName + "'/>");
        base.Render(writer);
    }

    #region IPostBackDataHandler Members

    public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
    {
        bool gunDegisti=false, ayDegisti=false, yilDegisti=false;

        string seciliGun = postCollection[this.UniqueID+"Gun"];
        if (SeciliGun != seciliGun)
        {
            SeciliGun = seciliGun;
            gunDegisti = true;
        }
        string seciliAy = postCollection[this.UniqueID + "Ay"];
        if (SeciliAy != seciliAy)
        {
            SeciliAy = seciliAy;
            ayDegisti = true;
        }
    
        string seciliYil = postCollection[this.UniqueID + "Yil"];
        if (SeciliYil != seciliYil)
        {
            SeciliYil = seciliYil;
            yilDegisti = true;
        }
        return gunDegisti||ayDegisti||yilDegisti;
    }

    public void RaisePostDataChangedEvent()
    {
        TarihKontrolumEventDatas tded = new TarihKontrolumEventDatas(SeciliGun, SeciliAy, SeciliYil);
        OnTarihDogrula(tded);
    }

    #endregion
}
```

Kontrolümüzü bu şekilde bir web sayfasında tekrardan test edelim. Bu kez LoadPostData, RaisePostDataChangedEvent, OnTarihDogrula ve web sayfamızdaki TarihKontrolumWithEvent1_TarihDogrula metodlarına birer breakpoint koyarak debug mode üzerinden kontrolümüzü izlemeye çalışalım. Kontrol üzerindeki submit düğmemize bastığımızda sayfa sunucuya geldikten sonra, kontrol ile ilişkili olarak ilk LoadPostData metoduna girilecektir. Burada NameValueCollection tipinden olan postCollection değişkeninin içeriğine baktığımızda aşağıdaki ekran görüntüsündekine benzer bir sonuç ile karşılaşırız.

![mk190_6.gif](/assets/images/2007/mk190_6.gif)

Dikkat ederseniz debug modda postCollection isimli değişkenin içeriğinden, istemci tarafında değiştirilen Gun, Ay ve Yil değerleri elde edilebilmektedir. Yani istemciden gelen bu kontole ait form verilerini çekebiliriz.

> Dikkat ederseniz Render metodu içerisinde, Select elementlerinin name attribute'larına değer verirken this.UniqueId özelliğinden faydalanmıştır. Bunun sonucu olarak istemci tarafına giden select elementlerinin adları içerisinde dahil oldukları kontrolün unique id değeride yer alacaktır. Eğer bunu yapmassak, aynı web sayfası içerisinde bu kontrollerden birden fazlasını kullandığımızda, LoadPostData metoduna gelen koleksiyon bilgisinden, hangi kontrol için hangi değerlerin geldiğini ayrıştıramayız.

LoadPostData metodu içerisinde bir dizi kontrol işlemi yapılmaktadır. Sonuç itibariyle form ile gelen verilerin sayfanın ViewState'i içerisinde tutulan verilerden farklı olmaması halinde boşu boşuna olay metodunu çalıştırmayı istemediğimiz için bu kontroller yapılmaktadır. Ancak Gun, Ay yada Yil değerlerini taşıyan select elementlerinden birisinde olacak bir değişiklik, RaisePostDataChanged metodunun çağırılmasını da sağlayacaktır.

Debug modda ilerlediğimiz takdirde,(eğer kontrol içerisindeki değerlerde değişiklik varsa), RaisePostDataChanged olay metoduna girilecektir. Sonrasında ise bu metod içerisinde yaptığımız çağrı sayesinde OnTarihDogrula metoduna geçilecektir. OnTarihDogrula metodu dikkat ederseniz, kontrol için bir TarihDogrula olayının yüklenip yüklenmediğini kontrol etmektedir. Eğer yüklü ise, (yani sayfa geliştirici tarafından kontrole ait bu olay metodu yazılmışsa) TarihKontrolumWithEvent1_TarihDogrula isimli olay metodundaki kodlar çalıştırılacaktır. Bu kodlarda basit olarak yapılan, olay verilerini alıp ekrana bastırmaktır. Bunu test kodu olarak yazmış bulunuyoruz. Özetle kontrolümüz aşağıdaki gibi çalışabilecektir.

Gördüğünüz gibi artık kontrolümüzde değişiklikeri algılayabiliyor, bunları herhangibir olay içerisinde kullanabiliyoruz. UniqueId özelliğinden yararlanmamızın doğal sonucu olarak, sayfa üzerinde birden fazla kontrol kullandığımız takdirde de sistemin sorunsuz bir şekilde çalışabileceğini görebiliriz.

Bazı durumlarda PostBack verilerini ele almak istemiyor olabiliriz. Yani amaç sadece istemciden sunucuya doğru olay tetikletmek olabilir. Bu gibi durumlarda ilgili olayları tetikleyebilmek için IPostBackEventHandler arayüzünden faydalanabiliriz. Bu arayüz, uygulandığı kontrol içerisinde RaisePostBackEvent metodunun yazılmasını zorlayacaktır. Bu makalemizde işlediklerimizden yanımıza kar kalan unsurlar aşağıdaki tabloda yer almaktadır.

Yanımıza Kar Kalanlar

Web kontrollerimize kendi olaylarımızı (event) öğretmek için kendi temsilcilerimizden faydalanabilir yada.Net Framework içerisinde var olan olay temsilcilerinden yararlanabiliriz.

Bir web kontrolünde, istemciden sunucuya postback işlemini gerçekleştirilmesi için Submit işlemini üstlenen elementin adının, kontrolün UniqueId değeri ile aynı olması gerekir.

Form üzerinden gelen verilerin (form-datas) ele alınabilmesi için, submit elementinin adının, kontrolün adı ile aynı olması yeterli değildir. Bu verileri okuyabilmek ve sonrasında ilgili olayları tetikleyebilmek için IPostBackDataHandler arayüzünün, web kontrolüne uygulanması gerekmektedir.

Sadece olay tetikletmek amacıyla, IPostBackEventHandler arayüzünün ilgili kontrole uygulanmasıda düşünülebilir. Elbetteki hem IPostBackDataHandler hemde IPostBackEventHandler arayüzlerinin bir arada uygulanması daha güçlü bir web kontrolü oluşturulmasına neden olacaktır.

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.