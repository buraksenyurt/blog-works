---
title: "Localization (Yerelleştirme) 2 - Dil Desteği"
pubDate: 2004-08-07 12:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - XML
  - Threading
  - Reflection
  - Visual Studio
---

# Localization (Yerelleştirme) 2 - Dil Desteği
Değerli Okurlarım, Merhabalar.

Bir önceki makalemizde hatırlayacağınız gibi,.net ile geliştirilen uygulamaların belirli kültürler için nasıl yerelleştirilebileceğini incelemeye başlamıştık. İlk bölümde, belirli bir kültürün daha çok sayısal, tarihsel ve sıralama formatları üzerinde durduk. Bu bölümde ise, yerelleştirmede daha da önemli olan bir konuya, uygulamaların farklı dillere göre destek vermesine değineceğiz.

Bir.net uygulamasının farklı dillere destek vermesindeki anahtar nokta, Resource dosyalarının etkin bir şekilde kullanılmasında yatmaktadır. Örneğin, uygulamamızın arayüzünün Türkçe olarak oluşturulduğunu düşünelim. Bu uygulamaya ait arayüzdeki metinleri, başka bir dilde sunabilmek için, ilgili kelimelerin diğer dildeki karşılıklarının bir şekilde assembly tarafından bilinmesi gerekir. Sadece bilinmesi elbette yeterli değildir. Ayrıca, çalışan proses için, arayüz kültürününde istenen dile ayarlanması gerekmektedir.

Şimdi dilerseniz bu işlemlerin nasıl gerçekleştirilebildiğini basit bir uygulama üzerinde anlamaya çalışalım. Uygulamamız aşağıdaki form görüntüsüne sahip bir windows uygulaması olacak. Şu anda form üzerindeki tüm metinler Türkçe. Biz uygulamamızı, İngilizce, Fransızca ve Almanca dillerine destek verecek hale getireceğiz.

![mk82_1.gif](images/mk82_1.gif)

Şekil 1. Uygulama Form Tasarımımız.

İlk olarak Resource dosyalarını oluşturacağız. Bu dosyaları, birer sözlük olarak düşünebiliriz. Resource dosyaları XML tabanlı dosyalar olup, temelde verileri anahtar yada isim (key - name) - değer (value) çiftleri şeklinde tutmaktadırlar. Bu açıdan bakıldıklarında bunları Hash Tablolarına benzetebiliriz. Dolayısıyla, uygulamamıza dil desteği sağlamak istiyorsak, ortak isimler (names) belirleyip, ilgili dil için uygun değerleri atayacağımız dosyalar oluşturmamız gerekecektir. Yani Türkçe için bir tane, İngilizce için bir tane, Almanca için bir tane ve Fransızca için bir tane. Bir Resource dosyasını Vs.Net ortamında uygulamaya eklemek için projeye sağ tıklayıp Add New Item iletişim kutusuna girmemiz ve Assembly Resource File dosya tipini seçmemiz yeterlidir. Burada önemli olan Resource dosyasının isimlendiriliş şeklidir.

![mk82_2.gif](images/mk82_2.gif)

Şekil 2. Resource Dosyasının Eklenmesi.

Dikkat edecek olursanız dosyamızı isimlendiriken tr-TR takısını kullandık. Bu Türkçe dilini konuşan Ülkemizi temsil eden belirleyici bir kültür kodudur. Dosyanın bu şekilde isimlendirilmesinin sebebi, çalışma zamanında, seçilen kültüre göre hangi Resource dosyasındaki kelimelerin yükleneceğinin belirlenebilmesidir. Bunu kodlarımızı yazdığımızda daha net bir şekilde açıklamaya çalışacağım. Şimdi dosyamızın name ve value değerlerini girelim.

![mk82_3.gif](images/mk82_3.gif)

Şekil 3. Resource1.tr-TR.resx dosyasının içeriği.

Artık tek yapmamız gereken, diğer diller içinde gerekli olan resx dosyalarını oluşturmak. Bu dosyalarda, name yada key değerlerimiz Türkçe olacak. Ancak karşılıkları olan kelimeler value değeri olarak belirlenecek. Örneğin, Fransızca için aşağıdaki bilgiler girilecek.

![mk82_4.gif](images/mk82_4.gif)

Şekil 4. fr-Fr kültürü için dil bilgilerinin Resource dosyasına girilmesi.

Sonuç olarak, projemizde aşağıdaki Resource dosyalarının oluşturulması gerekmektedir. Bu sayede, uygulamamız dünyanın neresinde olursa olsun, kaynak dosyalarda belirtilen dillere ve kültürlere destek verebilecektir. Elbette, her resx dosyasında name alanlarına yazılan Türkçe kelimelerin diğer dildeki karşılıkları value alanına girilecektir.

![mk82_5.gif](images/mk82_5.gif)

Şekil 5. Resx dosyalarımız.

Şimdi uygulama kodlarımızı yazmaya başlayalım. Uygulamamız için, yerelleştirme söz konusu olduğundan, System.Globalization isim alanını ve proses için kültürel özellikleri ve dil özelliklerini belirleyeceğimizden System.Threading isim alanlarını ilk olarak eklemeliyiz. Bunların yanında, assembly içindeki Resource dosyalarını yönetebilmemiz için, ResourceManager sınıfından bir nesneye ihtiyacımız olacaktır. Bu nesne içinde, System.Resources isim alanını uygulamamıza eklemeliyiz. İşte kodlarımız,

```csharp
private void Doldur()
{
    this.textBox1.Text=DateTime.Today.ToLongDateString();
    this.textBox2.Text=DateTime.Now.ToLongTimeString();
    double sayi=121345.4565;
    this.textBox3.Text=sayi.ToString();
}
private void Form1_Load(object sender, System.EventArgs e)
{
    Doldur();
}

private void button1_Click(object sender, System.EventArgs e)
{
    if(comboBox1.SelectedItem.ToString()=="USA")
    {
        Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture=new CultureInfo("en-US");
    }
    else if(comboBox1.SelectedItem.ToString()=="Türkiye")
    {
        Thread.CurrentThread.CurrentUICulture=new CultureInfo("tr-TR");
        Thread.CurrentThread.CurrentCulture=new CultureInfo("tr-TR");
    } 
    else if(comboBox1.SelectedItem.ToString()=="Français")
    {
        Thread.CurrentThread.CurrentUICulture=new CultureInfo("fr-FR");
        Thread.CurrentThread.CurrentCulture=new CultureInfo("fr-FR");
    }
    else if(comboBox1.SelectedItem.ToString()=="Deutschland")
    {
        Thread.CurrentThread.CurrentUICulture=new CultureInfo("de-DE");
        Thread.CurrentThread.CurrentCulture=new CultureInfo("de-DE");
    }

    ResourceManager resM=new ResourceManager("Languages.Resource1",Type.GetType("Languages.Form1").Assembly);
    this.Text=resM.GetString("Türkçe Dil");
    this.lblDil.Text=resM.GetString("Dil");
    this.lblParasal.Text=resM.GetString("Parasal");
    this.lblSaat.Text=resM.GetString("Saat");
    this.lblTarih.Text=resM.GetString("Tarih");
    this.button1.Text=resM.GetString("Göster");
    Doldur();
}
```

Kullanıcı comboBox1 kontrolünden bir dil seçtiğinde, öncelikle uygulamanın arayüzünün hangi dili kullanacağı CurrentUICulture özelliği ile belirlenmektedir. İşte bu noktada, resource dosyalarımız isimlendirilirken neden kültür kodlarının kullanıldığı dahada belirginleşmektedir. Nitekim, bu özellik, CultureInfo türünden bir nesne alır. Bu nesne belirli bir kültür için oluşturulmaktadır. Bu kültürün kodu, uygulama tarafından Resource dosyalarının isimlerinde aranır. Yani kullanıcı USA'i seçtiğinde, uygulama arayüzü için en-US kültür kodu belirlenir. Dolayısıyla, ResourceManager nesnesi, bu kültür kodunu içeren Resource dosyasını kullanmaya başlayacaktır.

If koşullarında, sonraki hamlede, güncel proses'deki sayısal,tarihsel vb... formatlar için gerekli kültür kodu, CurrentCulture özelliğine, ilgili kültür için bir CultureInfo nesnesi atanarak gerçekleştirilir. Bizim için önemli olan bir diğer noktada, ResourceManager nesnesinin oluşturuluş şeklidir.

```csharp
ResourceManager resM=new ResourceManager("Languages.Resource1",Type.GetType("Languages.Form1").Assembly);
```

Burada ilk parametre, Resource dosyalarının ana adını işaret etmektedir. Ana adımız örneğin Resource1.tr-TR.resx dosyasının baz aldığımızda, belirleyici kültür koduna kadar olan kısımdaki dosya adıdır. İkinci parametre ise, Reflection özelliklerini kullanır ve güncel assembly'ın tipini alır. Artık elimizde, resource'ları yönetebileceğimiz bir nesne vardır. Tek yapmamız gereken, uygulama arayüzündeki text'lere, ResourceManager nesnesinin GetString metodu ile, name (key) alanlarının karşılığı olan değerlerin (value) atanmasıdır. Bunun için, ResourceManager sınıfının GetString metodunu kullandık.

Şimdi akla şu soru gelebilir. ResourceManager hangi resx dosyasını, dolayısıyla hangi dili kullanacağını nereden bilecek. İşte bunu sağlayan, az önce bahsettiğimiz CurrentThread sınıfının CurrentUICulture özelliğidir. Bu özelliğe atadığımız belirleyici kod burada devreye girerek, ResourceManager'a hangi resx dosyasını kullanması gerektiğini söylemektedir. Bu açıklamalardan sonra dilerseniz, uygulamamızı derleyip çalıştıralım. Her hangibir dili seçtiğimizde, hem formatların hemde arayüzdeki metinlerin ilgili dile göre değiştiğini görürüz. İşte arayüzdeki metinlerin seçilen dildeki karşılıkları, oluşturduğumuz resx dosyalarından, ResourceManager sınıfının GetString metodu ile alınmaktadır.

![mk82_7.gif](images/mk82_7.gif)

Şekil 6. Almanca.

![mk82_6.gif](images/mk82_6.gif)

Şekil 7. İngilizce.

![mk82_8.gif](images/mk82_8.gif)

Şekil 8. Fransızca

Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[Örnek uygulama için tıklayın](assets/Languages.zip)
