---
layout: post
title: "ArrayList Koleksiyonu ve DataGrid"
date: 2004-01-07 12:00:00 +0300
description: "Bugünkü makalemizde, veritabanlarındaki tablo yapısında olan bir ArrayList'i bir DataGrid kontrolüne nasıl veri kaynağı olarak bağlayacağımızı inceleyeceğiz. Bildiğiniz gibi ArrayList bir koleksiyon sınıfıdır ve System.Collections isim uzayında yer almaktadır. Genelde ArrayList koleksiyonlarını tercih etmemizin nedeni, dizilere olan üstünlüklerinden kaynaklanmaktadır."
categories:
  - csharp
tags:
  - csharp
---
# ArrayList Koleksiyonu ve DataGrid
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde, veritabanlarındaki tablo yapısında olan bir ArrayList'i bir DataGrid kontrolüne nasıl veri kaynağı olarak bağlayacağımızı inceleyeceğiz. Bildiğiniz gibi ArrayList bir koleksiyon sınıfıdır ve System.Collections isim uzayında yer almaktadır. Genelde ArrayList koleksiyonlarını tercih etmemizin nedeni, dizilere olan üstünlüklerinden kaynaklanmaktadır.

En büyük tercih nedeni, normal dizilerin boyutlarının çalışma esnasında değiştirilemeyişidir. Böyle bir işlemi gerçekleştirmek için, dizi elemanları yeni boyutlu başka boş bir diziye kopyalanır. Oysaki, ArrayList koleksiyonunda böyle bir durum söz konusu değildir. Koleksiyonu, aşağıdaki yapıcı metodu ile oluşturduğunuzda boyut belirtmezsiniz. Eleman ekledikçe, ArrayList'in kapasitesi otomatik olarak büyüyecektir.

public ArrayList ();

Bu şekilde tanımlanan bir ArrayList koleksiyonu varsayılan olarak 16 elemalı bir koleksiyon dizisi olur. Eğer kapasite aşılırsa, koleksiyonun boyutu otomatik olarak artacaktır. Bu elbette karşımıza 17 elemanlı bir koleksiyonumuz varsa fazladan yer harcadığımız anlamınada gelmektedir. Ancak sorunu TrimToSize metodu ile halledebiliriz. Dilerseniz bu konuyu aşağıdaki basit console uygulaması ile açıklayalım.

```csharp
using System;
using System.Collections;

namespace TrimToSizeArray
{
    class Class1
    {
        static void Main(string[] args)
        {
            ArrayList list = new ArrayList();
            Console.WriteLine("ArrayList'in başlangıçtaki kapasitesi " + list.Capacity.ToString()); /*Capacity koleksiyonun üst eleman limitini verir. */

            Console.WriteLine("--------");

            for (int i = 1; i <= 15; ++i)
            {
                list.Add(i);
                /* ArrayList koleksiyonunun sonuna eleman ekler. */
            }

            Console.WriteLine("ArrayList'in güncel eleman sayısı " + list.Count.ToString());

            /* Count özelliği koleksiyonun o anki eleman sayısını verir. */
            Console.WriteLine("ArrayList'in güncel kapasitesi " + list.Capacity.ToString());

            Console.WriteLine("--------");

            for (int j = 1; j < 8; ++j)
            {
                list.Add(j);
            }
            Console.WriteLine("ArrayList'in güncel eleman sayısı " + list.Count.ToString());
            Console.WriteLine("ArrayList'in güncel kapasitesi " + list.Capacity.ToString());
            Console.WriteLine("--------");
            list.TrimToSize();
            /* TrimToSize dizideki eleman sayısı ile kapasiteyi eşitler. */
            Console.WriteLine("TrimToSize sonrası:");
            Console.WriteLine("ArrayList'in güncel eleman sayısı " + list.Count.ToString());
            Console.WriteLine("ArrayList'in güncel kapasitesi " + list.Capacity.ToString());
        }
    }
}
```

Bu örneği çalıştırdığımızda aşağıdaki sonucu elde ederiz.

![mk37_1.gif](/assets/images/2004/mk37_1.gif)

Şekil 1. TrimToSize'ın etkisi.

Görüldüğü gibi koleksiyonumuz ilk oluşturulduğunda kapasitesi 16'dır. Daha sonra koleksiyona 15 eleman ekledik. Halen koleksiyonun kapasite limitleri içinde olduğumuzdan, kapasitesi 16 dır. Ancak sonra 8 eleman daha ekledik. 17nci eleman koleksiyona girdiğinde, koleksiyonun kapasitesi otomatik olarak iki katına çıkar. Bu durumda Capacity özelliğimiz 32 değerini verecektir. TrimToSize metodunu uyguladığımızda koleksiyonun kapasitesinin otomatik olarak eleman sayısı ile eşleştrildiğini görürüz.

Örneğimizde koleksiyonumuza eleman eklemek için Add metodunu kullandık. Add metodu her zaman yeni elemanı koleksiyonun sonuna ekler. Eğer koleksiyonda araya eleman eklemek istiyorsak insert metodunu, koleksiyonumuzdan bir eleman çıkartmak istediğimizde ise Remove metodunu kullanırız. Insert metodunun prototipi aşağıdaki gibidir.

public virtual void Insert (int index,object value);

İlk parametremiz 0 indeks tabanlı bir değerdir ve object tipindeki ikinci parametre değerinin hangi indeksli eleman olarak yerleştirileceğini belirtmektedir. Dolayısıyla bu elemanın insert edildiği yerdeki eleman bir ileriye ötelenmiş olucaktır.

Remove metodu ise belirtilen elemanı koleksiyondan çıkartmaktadır. Prototipi aşağıdaki gibidir.

public virtual void Remove (object obj);

Metodumuz direkt olarak, çıkartılmak istenen elemanın değerini alır. ArrayList koleksiyonu, Remove metoduna alternatif başka metodlarada sahiptir. Bunlar, RemoveAt ve RemoveRange metodlarıdır. RemoveAt metodu parametre olarak bir indeks değeri alır ve bu indeks değerindeki elemanı koleksiyondan çıkartır. Eğer girilen indeks değeri 0 dan küçük yada koleksiyonun eleman sayısına eşit veya büyük ise ArgumentOutOfRangeException istisnası fırlatılır.

RemoveRange metodu ise, ilk parametrede belirtilen indeks'ten, ikinci parametrede belirtilen sayıda elemanı koleksiyondan çıkartır. Elbette eğer indeks değeri 0 dan küçük yada koleksiyonun eleman sayısına eşit veya büyük ise ArgumentOutOfRangeException istisnası alınır. Tabi girdiğimiz ikinci parametre değeri, çıkartılmak istenen eleman sayısını, indeksten itibaren ele alındığında, koleksiyonun count özelliğinin değerinin üstüne çıkabilir. Bu durumda ise ArgumentException istisnası üretilecektir.

public virtual void RemoveAt (int index);

public virtual void RemoveRange (int index,int count);

Şimdi dilerseniz bu metodları küçük bir console uygulaması ile deneyelim.

```csharp
using System;

using System.Collections;

namespace TrimToSizeArray
{
    class Class1
    {
        static void Main(string[] args)
        {
            ArrayList kullanicilar = new ArrayList(); /* ArrayList koleksiyonumuz oluşturuluyor. */

            /* ArrayList koleksiyonumuza elemanlar ekleniyor. */
            kullanicilar.Add("Burki");
            kullanicilar.Add("Selo");
            kullanicilar.Add("Melo");
            kullanicilar.Add("Alo");
            kullanicilar.Add("Neo");
            kullanicilar.Add("Ceo");
            kullanicilar.Add("Seko");
            kullanicilar.Add("Dako");
            /* Foreach döngüsü yardımıyla, koleksiyonumuz içindeki tüm elemanları ekrana yazdırıyoruz. Elemanların herbirinin object tipinden ele alındığına dikkat edin. */

            foreach (object k in kullanicilar)
            {
                Console.Write(k.ToString() + "|");
            }

            Console.WriteLine();
            Console.WriteLine("-----");
            kullanicilar.Insert(3, "Melodan Sonra");
            /* 3 noldu indeks'e "Melodan Sonra" elemanını yerleştirir. Bu durumda, "Alo" isimli eleman ve sonrakiler bir ileriye kayarlar. */

            foreach (object k in kullanicilar)
            {
                Console.Write(k.ToString() + "|");
            }
            Console.WriteLine();
            Console.WriteLine("-----");
            kullanicilar.Remove("Melodan Sonra");
            /* "Melodan Sonra" isimli elemanı koleksiyondan çıkartır. */

            foreach (object k in kullanicilar)
            {
                Console.Write(k.ToString() + "|");
            }
            Console.WriteLine();
            Console.WriteLine("-----");
            kullanicilar.RemoveAt(2);
            /* 2nci indeks'te bulunan eleman koleksiyondan çıkartılır. Yani "Melo" çıkartılır. */

            foreach (object k in kullanicilar)
            {
                Console.Write(k.ToString() + "|");
            }
            Console.WriteLine();
            Console.WriteLine("-----");
            kullanicilar.RemoveRange(3, 2);
            /* 3ncü indeks'ten itibaren, 2 eleman koleksiyondan çıkartılır. Yani "Neo" ve "Ceo" koleksiyondan çıkartılır. */
            foreach (object k in kullanicilar)
            {
                Console.Write(k.ToString() + "|");
            }
            Console.WriteLine();
            Console.WriteLine("-----");
        }
    }
}
```

Uygulamamızı çalıştırdığımızda aşağıdaki ekran görüntüsünü elde ederiz.

![mk37_2.gif](/assets/images/2004/mk37_2.gif)

Şekil 2. Insert,Remove,RemoveAt ve RemoveRange metodları.

ArrayList koleksiyonu ile ilgili bu bilgilerden sonra sıra geldi DataGrid ile ilişkili olan kısma. Bildiğiniz gibi ArrayList'ler tüm koleksiyon sınıfları gibi elemanları object olarak tutarlar. Dolayısıyla bir sınıf nesnesinide bir ArrayList koleksiyonuna eleman olarak ekleyebiliriz. Şimdi geliştireceğimiz örnek uygulamada, bir veri tablosu gibi davranan bir ArrayList oluşturacağız. Bir veritablosu gibi alanları olucak. Peki bunu nasıl yapacağız?

Öncelikle, tablodaki her bir alanı birer özellik olarak tutacak bir sınıf tanımlayacağız. Bu durumda, bu sınıftan türettiğimiz her bir nesnede sahip olduğu özellik değerleri ile, tablodaki bir satırlık veriyi temsil etmiş olucak. Daha sonra bu nesneyi, oluşturduğumuz ArrayList koleksiyonuna ekleyeceğiz. Son olarakta bu ArrayList koleksiyonunu, DataGrid kontrolümüze veri kaynağı olarak bağlıyacağız.

Öncelikle Formumuzu tasarlayalım.

![mk37_3.gif](/assets/images/2004/mk37_3.gif)

Şekil 3. Form Tasarımımız.

Şimdi koleksiyonumuzun taşıyacağı nesnelerin sınıfını tasarlayalım.

```csharp
using System;
using System.Collections;
namespace ArrayListAndDataGrid
{
    public class MailList
    {
        public MailList(string k, string m)
        {
            mailAdresi = m;
            kullanici = k;
        }
        public MailList()
        {
        }
        protected string mailAdresi;
        protected string kullanici;
        public string MailAdresi
        {
            get
            {
                return mailAdresi;
            }
            set
            {
                mailAdresi = value;
            }
        }
        public string Kullanici
        {
            get
            {
                return kullanici;
            }
            set
            {
                kullanici = value;
            }
        }
    }
}
```

MailList sınıfımız Kullanici ve MailAdresi isimli iki özelliğe sahip. İşte bunlar tablo alanlarımızı temsil etmektedir. Şimdi program kodlarımızı yazalım.

```csharp
using System.Collections;
using System;

ArrayList mList; /* ArrayList koleksiyonumuzu tanımlıyoruz. */

private void Form1_Load(object sender, System.EventArgs e)
{
    mList = new ArrayList(); /* Form yüklenirken, ArrayList koleksiyonumuz oluşturuluyor. */
    lblElemanSayisi.Text = mList.Count.ToString();

    /* Koleksiyonumuzun eleman sayısı alınıyor ve label kontrolüne yazdırılıyor. */
}

private void btnEkle_Click(object sender, System.EventArgs e)
{
    if (txtKullanici.Text.Length == 0 && txtMail.Text.Length == 0)
    {
        MessageBox.Show("Lütfen veri girin");
    }
    else
    {
        MailList kisi = new MailList(txtKullanici.Text, txtMail.Text); /* MailList sınıfından bir nesne oluşturuluyor. Yapıcı metodumuz parametre olarak, textBox kontrollerine girilen değerleri alıyor ve bunları ilgili alanlara atıyor. */

        mList.Add(kisi);
        /* MailList sınıfından oluşturduğumuz nesnemizi koleksiyonumuza ekliyoruz. */
        dgListe.DataSource = null; /* DataGrid' veri kaynağı olarak önce null değer atıyor. Nitekim, koleksiyonumuza her bir sınıf nesnesi eklendiğinde, koleksiyon güncellenirken, dataGrid'in güncellenmediğini görürüz. Yapılacak tek şey veri kaynağını önce null olarak ayarlamaktır. */
        dgListe.Refresh();
        dgListe.DataSource = mList;

        /* Şimdi işte, dataGrid kontolümüze veri kaynağı olarak koleksiyonumuzu bağlıyoruz. */
        dgListe.Refresh();
        lblElemanSayisi.Text = mList.Count.ToString();
        txtKullanici.Clear();
        txtMail.Clear();
    }
}

private void btnSil_Click(object sender, System.EventArgs e)
{
    int index;
    index = dgListe.CurrentCell.RowNumber; /* Silinmek istenen eleman DataGrid'te seçildiğinde, hangi satırın seçildiğini öğrenmek için CurrentCell.RowNumber özelliğini kullanıyoruz. */

    try
    {
        MailList kisi = new MailList(); /* Bir MailList nesnesi oluşturuyoruz. */
        kisi = (MailList)mList[index];
        /* index değerimiz, DataGrid kontrolünde, bizim seçtiğimiz satırın, koleksiyonda karşılık gelen index değeridir. mList koleksiyonunda ilgili indeks'teki elemanı alıp (MailList) söz dizimi ile MailList tipinden bir nesneye dönüştürüyoruz. Nitekim koleksiyonlar elemanlarını object olarak tutarken, bu elemanları dışarı alırken açıkça bir dönüştürme işlemi uygulamamız gerekir. */
        mList.Remove(kisi);
        /* Remove metodu ile, kisi nesnemiz, dolayısıyla dataGrid kontrolünde seçtiğimiz eleman koleksiyondan çıkartılıyor. */

        dgListe.DataSource = null;
        dgListe.Refresh();
        dgListe.DataSource = mList;
        dgListe.Refresh();
        lblElemanSayisi.Text = mList.Count.ToString();
    }
    catch (Exception hata)
    {
        MessageBox.Show(hata.Message.ToString());
    }
}
```

Şimdi uygulamamızı çalıştıralım.

![mk37_4.gif](/assets/images/2004/mk37_4.gif)

Şekil 4. Programın Çalışması

Bir sonraki adımımız bu ArrayList'in sahip olduğu verilerin gerçek bir tabloya yazdırılması olabilir. Bu adımın geliştirilesini siz değerli Okurlarıma bırakıyorum. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.