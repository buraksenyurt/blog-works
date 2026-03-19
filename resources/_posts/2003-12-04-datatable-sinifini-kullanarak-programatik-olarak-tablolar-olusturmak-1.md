---
layout: post
title: "DataTable Sınıfını Kullanarak Programatik Olarak Tablolar Oluşturmak-1"
date: 2003-12-04 12:00:00 +0300
description: "Bugünkü makalemizde bağlantısız katmanın önemli bir sınıfı olan DataTable nesnesini bir açıdan incelemeye çalışacağız. Bilindiği gibi DataTable sınıfından türetilen bir nesne, bir tabloyu ve elemanlarını bellekte temsil etmek için kullanılmaktadır. DataTable sınıfı bellekte temsil ettiği tablolara ait olan satırları Rows koleksiyonuna ait DataRow nesneleri ile temsil ederken, tabloun alanlarını ise, Columns koleksiyonuna ait DataColumn nesneleri ile temsil etmektedir."
categories:
  - ado-net
tags:
  - ado-net
  - csharp
  - datatable
---
# DataTable Sınıfını Kullanarak Programatik Olarak Tablolar Oluşturmak-1
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde bağlantısız katmanın önemli bir sınıfı olan DataTable nesnesini bir açıdan incelemeye çalışacağız. Bilindiği gibi DataTable sınıfından türetilen bir nesne, bir tabloyu ve elemanlarını bellekte temsil etmek için kullanılmaktadır. DataTable sınıfı bellekte temsil ettiği tablolara ait olan satırları Rows koleksiyonuna ait DataRow nesneleri ile temsil ederken, tabloun alanlarını ise, Columns koleksiyonuna ait DataColumn nesneleri ile temsil etmektedir.

Örnek uygulamamızda bu sınıf nesnelerini detaylı olarak kullanacağız. Diğer yandan DataTable sınıfı bir tabloya ilişkin kıstasların yer aldığı Constraints koleksiyonuna ait Constraint nesnelerinedee sahiptir. DataTable sınıfının ve üye elemanlarını aşağıdaki şekilde daha kolayca canlandırabiliriz.

![mk13_1.gif](/assets/images/2003/mk13_1.gif)

Şekil 1 DataTable mimarisi

Geliştireceğimiz uygulamada, bizim belirlediğimiz alanlardan oluşan bir tabloyu bellekte oluşturmaya çalışacağız. Öncelikle DataTable nesnesi ile bir tablo oluşturmak için aşağıdaki adımları takip etmeliyiz.

Bir DataTable nesnesi oluşturup DataTable’ın bellekte temsil edeceği tablo için bir isim belirlenir.
Tablomuzun içereceği alanların isimleri, veri türleri belirlenerek birer DataRow nesnesi şeklinde, DataTable nesnesinin Columns koleksiyonuna eklenir.
Tablomuz için bir primary key alanı belirlenir.

Bu adımların ardından tablomuz bellekte oluşturulmuş olucaktır. Bu noktadan sonra bu tablo üzerinde dilediğimiz işlemleri yapabiliriz. Kayıt ekleyebilir, silebilir, sorgulayabiliriz. Ama tabiki programı kapattığımızda bellekteki tablonun yerinde yeller esiyor olucaktır. Ama üzülmeyin ilerliyen makalelerimizde SQL-DMO komutları yardımıyla programımız içinden bir sql sunucusu üzerinde veritabanı oluşturacak ve tablomuzu buraya ekleyeceğiz.Şimdi dilerseniz birinci adımdan itibaren bu işlerin nasıl yapıldığını minik örnekler ile inceleyelim ve daha sonrada asıl uygulamamaızı yazalım. Öncelikle işe tablomuzu bellekte temsil edicek datatable nesnesi oluşturarak başlayalım. Aşağıdaki küçük uygulamayı oluşturalım.

![mk13_2.gif](/assets/images/2003/mk13_2.gif)

Şekil 2. Formun ilk hali

Ve kodlar,

```csharp
private void btnTabloOlustur_Click(object sender, System.EventArgs e)
{
            /* Bir tabloyu bellekte temsil edicek bir datatable nesnesi oluşturarak işe başlıyoruz. Tablomuza txtTabloAdi isimli TextBox'a giriline değeri isim olarak veriyoruz */
            DataTable dt=new DataTable(txtTabloAdi.Text);
            MessageBox.Show(dt.TableName.ToString()+" TABLOSU BELLEKTE OLUŞTURULDU");
} 
```

Şimdi programımızı çalıştıralım ve tablo ismi olarak DENEME diyelim. İşte sonuç,

![mk13_3.gif](/assets/images/2003/mk13_3.gif)

Şekil 3. DataTable nesnesi oluşturuldu.

Şimdi ise tablomuza nasıl field (alan) ekleyeceğimize bakalım. Önceden bahsettiğimiz gibi tablonun alanları aslında DataTable sınıfının Columns koleksiyonuna ait birer DataColumn nesnesidir. Dolayısıyla öncelikle bir DataRow nesnesi oluşturup bu nesneyi ilgili DataTable’ın Columns koleksiyonuna eklememiz gerekmektedir. Alanın ismi dışında tabiki veri türünüde belirtmeliyiz. Bu veri türlerini belirtirken Type.GetType syntaxı kullanılır. Formumuzu biraz değiştirelim. Kullanıcı belirlediği isimde ve türdeki alanı, tabloya ekleyebilecek olsun. Söylemek isterimki bu uygulamada hiç bir kontrol mekanizması uygulanmamış ve hataların önünce geçilmeye çalışılmamıştır. Nitekim amacımız DataTable ile bir tablonun nasıl oluşturulacağına dair basit bir örnek vermektir. Formumuzu aşağıdaki gibi değiştirelim. Kullanıcı bir tablo adı girip oluşturduktan sonra istediği alanları ekleyecek ve bu bilgiler listbox nesnemizde kullanıcıya ayrıca gösterilecek.

![mk13_4.gif](/assets/images/2003/mk13_4.gif)

Şekil 4. Formumuzun yeni hali.

Şimdide kodlarımızı görelim.

```csharp
DataTable dt; /* DataTable nesnemizi uygulama boyunca kullanabilmek için tüm metodların dışında tanımladık. */ 

private void btnTabloOlustur_Click(object sender, System.EventArgs e)
{
            dt=new DataTable(txtTabloAdi.Text);
            MessageBox.Show(dt.TableName.ToString()+" TABLOSU BELLEKTE OLUŞTURULDU");
            lblTabloAdi.Text=dt.TableName.ToString(); /* TableName özelliği DataTable nesnesinin bellekte temsil ettiği tablonun adını vermektedir.*/
}

private void btnAlanEkle_Click(object sender, System.EventArgs e)
{
            /*Önce yeni alanımız için bir DataColumn nesnesi oluşturulur*/
            DataColumn dc=new DataColumn();
            /*Şimdi ise DataTable nesnemizin Columns koleksiyonuna oluşturulan alanımızı ekliyoruz. İlk parametre, alanın adını temsil ederken, ikinci parametre ise alanın veri türünü belirtmektedir. Add metodu bu özellikleri ile oluşturulan DataColumn nesnesini dataTable'a ekler. Bu sayede tabloda alanımız oluşturulmuş olur.*/
           dt.Columns.Add(txtAlanAdi.Text,Type.GetType("System."+cmbAlanTuru.Text));
            lstAlanlar.Items.Add("Alan Adı: "+dt.Columns[txtAlanAdi.Text].ColumnName.ToString()+" Veri Tipi: "+dt.Columns[txtAlanAdi.Text].DataType.ToString()); 

            /* ColumnName özelliği ile eklenen alanın adını, DataType özelliği ilede bu alanın veri türünü öğreniyoruz.*/
}
```

Uygulamamızı deneyelim.

![mk13_5.gif](/assets/images/2003/mk13_5.gif)

Şekil 5. Alanlarımızıda tabloya ekleyelim.

Alanlara veri türlerini aktarırken kullanabileceğimiz diğer örnek değerler aşağıdaki gibidir; bunlar listbox kontrolüne desing time (tasarım zamanında) eklenmiştir.

```text
System.Int32
System.Int16
System.Int64
System.Byte
System.Char
System.Single
System.Decimal
System.Double
```

Gibi...

Şimdi de seçtiğimiz bir alanı primary key olarak belirleyelim. Unique (benzersiz) integer değerler alıcak bir alan olsun bu ve 1000 den başlayarak 1’er 1’er otomatik olarak artsın. Bildiğini ID alanlarından bahsediyorum. Bunu kullanıcıya sormadan otomatik olarak biz yaratalım ve konudan fazlaca uzaklaşmayalım. Sadece btnTabloOlustur’un kodlarına ekleme yapıyoruz.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde oluşturduğumuz bu tabloya nasıl veri ekleneceğini göreceğimiz çok kısa bir uygulama yazacağız. Hepinizi mutlu günler dilerim.