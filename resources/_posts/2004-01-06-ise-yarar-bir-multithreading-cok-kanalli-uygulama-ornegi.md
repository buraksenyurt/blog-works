---
layout: post
title: "İşe Yarar Bir MultiThreading(Çok Kanallı) Uygulama Örneği"
date: 2004-01-06 12:00:00 +0300
description: "Bundan önceki üç makalemizde iş parçacıkları hakkında bilgiler vermeye çalıştım. Bu makalemde ise işimize yarayacak tarzda bir uygulama geliştirecek ve bilgilerimizi pekiştireceğiz. Bir iş parçacığının belkide en çok işe yarayacağı yerlerden birisi veritabanı uygulamalarıdır. Bazen programımız çok uzun bir sonuç kümesi döndürecek sorgulara veya uzun sürecek güncelleme ifadeleri içeren sql cümlelerine sahip olabilir. Böyle bir durumda programın diğer öğeleri ile olan aktivitemizi devam ettirebilmek isteyebiliriz. Ya da aynı anda bir den fazla iş parçacığında, birden fazla veritabanı işlemini yaptırarak bu işlemlerin tamamının daha kısa sürelerde bitmesini sağlıyabiliriz. İşte bu gibi nedenleri göz önüne alarak bu gün birlikte basit ama faydalı olacağına inandığım bir uygulama geliştireceğiz."
categories:
  - csharp
tags:
  - csharp
  - threading
  - dataset
  - datatable
---
# İşe Yarar Bir MultiThreading(Çok Kanallı) Uygulama Örneği
Değerli Okurlarım, Merhabalar.

Bundan önceki üç makalemizde iş parçacıkları hakkında bilgiler vermeye çalıştım. Bu makalemde ise işimize yarayacak tarzda bir uygulama geliştirecek ve bilgilerimizi pekiştireceğiz. Bir iş parçacığının belkide en çok işe yarayacağı yerlerden birisi veritabanı uygulamalarıdır. Bazen programımız çok uzun bir sonuç kümesi döndürecek sorgulara veya uzun sürecek güncelleme ifadeleri içeren sql cümlelerine sahip olabilir. Böyle bir durumda programın diğer öğeleri ile olan aktivitemizi devam ettirebilmek isteyebiliriz. Ya da aynı anda bir den fazla iş parçacığında, birden fazla veritabanı işlemini yaptırarak bu işlemlerin tamamının daha kısa sürelerde bitmesini sağlıyabiliriz. İşte bu gibi nedenleri göz önüne alarak bu gün birlikte basit ama faydalı olacağına inandığım bir uygulama geliştireceğiz.

Olayı iyi anlayabilmek için öncelikle bir milat koymamız gerekli. İş parçacığından önceki durum ve sonraki durum şeklinde. Bu nedenle uygulamamızı önce iş parçacığı kullanmadan oluşturacağız. Sonrada iş parçacığı ile. Şimdi programımızdan kısaca bahsedelim. Uygulamamız aşağıdaki sql sorgusunu çalıştırıp, bellekteki bir DataSet nesnesinin referans ettiği bölgeyi, sorgu sonucu dönen veri kümesi ile dolduracak.

```text
SELECT Products.* From [Order Details] Cross Join Products
```

Bu sorgu çalıştırıldığında, Sql sunucusunda yer alan Northwind veritabanı üzerinden, 165936 satırlık veri kümesi döndürür. Elbette normalde böyle bir işlemi istemci makinenin belleğine yığmamız anlamsız. Ancak sunucu üzerinde çalışan ve özellikle raporlama amacı ile kullanılan sorguların bu tip sonuçlar döndürmeside olasıdır. Şimdi bu sorguyu çalıştırıp sonuçları bir DataSet'e alan ve bu veri kümesini bir DataGrid kontrolü içinde gösteren bir uygulama geliştirelim. Öncelikle aşağıdaki formumuzu tasarlayalım.

![mk36_1.gif](/assets/images/2004/mk36_1.gif)

Şekil 1. Form Tasarımımız.

Şimdide kodlarımızı yazalım.

```csharp
DataSet ds;

public void Bagla()
{
    dataGrid1.DataSource = ds.Tables[0];
}

public void Doldur()
{
    SqlConnection conNorthwind = new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=sspi");
    conNorthwind.Open();
    SqlDataAdapter daNorthwind = new SqlDataAdapter("SELECT Products.* From [Order Details] Cross Join Products", conNorthwind);
    ds = new DataSet();

    daNorthwind.Fill(ds);
    conNorthwind.Close();
    MessageBox.Show("DataTable dolduruldu...");
}

private void btnKapat_Click(object sender, System.EventArgs e)
{
    Close();
}

private void btnCalistir_Click(object sender, System.EventArgs e)
{
    Doldur();
}

private void btnGoster_Click(object sender, System.EventArgs e)
{
    Bagla();
}
```

Yazdığımız kodlar gayet basit. Sorgumuz bir SqlDataAdapter nesnesi ile, SqlConnection'ımız kullanılarak çalıştırılıyor ve daha sonra elde edilen veri kümesi DataSet'e aktarılıyor. Şimdi uygulamamızı bu haliyle çalıştıralım ve sorgumuzu Çalıştır başlıklı buton ile çalıştırdıktan sonra, textBox kontrolüne mouse ile tıklayıp bir şeyler yazmaya çalışalım.

![mk36_2.gif](/assets/images/2004/mk36_2.gif)

Şekil 2. İş parçacığı olmadan programın çalışması.

Görüldüğü gibi sorgu sonucu elde edilen veri kümesi DataSet'e doldurulana kadar TextBox kontrolüne bir şey yazamadık. Çünkü işlemcimiz satır kodlarını işletmek ile meşguldü ve bizim TextBox kontrolümüze olan tıklamamızı ele almadı. Demekki buradaki sorgumuzu bir iş parçacığı içinde tanımlamalıyız. Nitekim programımız donmasın ve başka işlemleride yapabilelim. Örneğin TextBox kontrolüne bir şeyler yazabilelim (bu noktada pek çok şey söylenebilir. Örneğin başka bir tablonun güncellenmesi gibi). Bu durumda yapmamız gereken kodlamayı inanıyorumki önceki makalelerden edindiğiniz bilgiler ile biliyorsunuzdur. Bu nedenle kodlarımızı detaylı bir şekilde açıklamadım. Şimdi gelin yeni kodlarımızı yazalım.

```csharp
DataSet ds;

public void Bagla()
{
     if(!t1.IsAlive)
     {
          dataGrid1.DataSource=ds.Tables[0];
     }
}

public void Doldur()
{
     SqlConnection conNorthwind=new SqlConnection("data source=localhost;initial catalog=Northwind;integrated security=sspi");
     conNorthwind.Open();

     SqlDataAdapter daNorthwind=new SqlDataAdapter("SELECT Products.* From [Order Details] Cross Join Products",conNorthwind);
     ds=new DataSet();
     daNorthwind.Fill(ds);
     conNorthwind.Close();
     MessageBox.Show("DataTable dolduruldu...");
}

ThreadStart ts1;
Thread t1;

private void btnKapat_Click(object sender, System.EventArgs e)
{
     if(!t1.IsAlive)
     {
          Close();
     }
     else
     {
          MessageBox.Show("Is parçacigi henüz sonlandirilmadi...Daha sonra tekrar deneyin.");
     }
}

private void btnCalistir_Click(object sender, System.EventArgs e)
{
     ts1=new ThreadStart(Doldur);
     t1=new Thread(ts1);
     t1.Start();
}

private void btnIptalEt_Click(object sender, System.EventArgs e)
{
     t1.Abort();
}

private void btnGoster_Click(object sender, System.EventArgs e)
{
     Bagla();
}
```

Şimdi programımızı çalıştıralım.

![mk36_3.gif](/assets/images/2004/mk36_3.gif)

Şekil 3. İş Parçacığının sonucu.

Görüldüğü gibi bu yoğun sorgu çalışırken TextBox kontrolüne bir takım yazılar yazabildik. Üstelik programın çalışması hiç kesilmeden. Şimdi Göster başlıklı butona tıkladığımızda veri kümesinin DataGrid kontrolüne alındığını görürüz.

![mk36_4.gif](/assets/images/2004/mk36_4.gif)

Şekil 4. Programın Çalışmasının Sonucu.

Geldik bir makalemizin daha sonuna. İlerliyen makalelerimizde Thred'leri daha derinlemesine incelemeye devam edeceğiz. Hepinize mutlu günler dilerim.