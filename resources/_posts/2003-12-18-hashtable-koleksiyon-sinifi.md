---
layout: post
title: "HashTable Koleksiyon Sınıfı"
date: 2003-12-18 12:00:00 +0300
description: "Bugünkü makalemizde HashTable koleksiyon sınıfını incelemeye çalışacağız. Bildiğiniz gibi Koleksiyonlar System.Collections namespace'inde yer almakta olup, birbirlerinin aynı veya birbirlerinden farklı veri tiplerinin bir arada tutulmasını sağlayan diziler oluşturmamıza imkan sağlamaktadırlar. Pek çok koleksiyon sınıfı vardır."
categories:
  - csharp
tags:
  - csharp
---
# HashTable Koleksiyon Sınıfı
Değerli Okurlarım, Merhabalar.

Bugünkü makalemizde HashTable koleksiyon sınıfını incelemeye çalışacağız. Bildiğiniz gibi Koleksiyonlar System.Collections namespace'inde yer almakta olup, birbirlerinin aynı veya birbirlerinden farklı veri tiplerinin bir arada tutulmasını sağlayan diziler oluşturmamıza imkan sağlamaktadırlar. Pek çok koleksiyon sınıfı vardır. Bugün bu koleksiyon sınıflarından birisi olan HashTable koleksiyon sınıfını inceleyeceğiz.

HashTable koleksiyon sınıfında veriler key-value dediğimiz anahtar-değer çiftleri şeklinde tutulmaktadırlar. Tüm koleksiyon sınıflarının ortak özelliği barındırdıkları verileri object tipinde olmalarıdır. Bu nedenle, HashTable'lardada key ve value değerleri herhangibir veri tipinde olabilirler. Temel olarak bunların her biri birer DictionaryEntry nesnesidir. Bahsetmiş olduğumuz key-value çiftleri hash tablosu adı verilen bir tabloda saklanırlar. Bu değer çiftlerine erişmek için kullanılan bir takım karmaşık kodlar vardır.

Key değerleri tektir ve değiştirilemezler. Yani bir key-value çiftini koleksiyonumuza eklediğimizde, bu değer çiftinin value değerini değiştirebilirken, key değerini değiştiremeyiz. Ayrıca key değerleri benzersiz olduklarında tam anlamıyla birer anahtar alan vazifesi görürler. Diğer yandan value değerline null değerler atayabilirken, anahtar alan niteliğindeki Key değerlerine null değerler atayamayız. Şayet uygulamamızda varolan bir Key değerini eklemek istersek ArgumentException istisnası ile karşılaşırız. HashTable koleksiyonu verilere hızı bir biçimde ulaşmamızı sağlayan bir kodlama yapısına sahiptir. Bu nedenle özellikle arama maliyetlerini düşürdüğü için tercih edilmektedir. Şimdi konuyu daha iyi pekiştirebilmek amacıyla, hemen basit bir uygulama geliştirelim.

Uygulamamızda, bir HastTable koleksiyonuna key-value çiftleri ekliyecek, belirtilen key'in sahip olduğu değere bakılacak, tüm HashTable'ın içerdiği key-value çiftleri listelenecek, eleman çiftlerini HashTable'dan çıkartacak vb... işlemler gerçekleştireceğiz. Form tasarımını ben aşağıdaki şekildeki gibi yaptım. Temel olarak teknik terimlerin türkçe karşılığına dair minik bir sözüğü bir HashTable olarak tasarlayacağız.

![mk22_1.gif](/assets/images/2003/mk22_1.gif)

1. Form Tasarımımız.

Şimdi kodlarımıza bir göz atalım.

```csharp
System.Collections.Hashtable htTeknikSozluk;

/* HashTable koleksiyon nesnemizi tanımlıyoruz.*/ 
private void Form1_Load(object sender, System.EventArgs e)
{
    htTeknikSozluk=new System.Collections.Hashtable(); /* HashTable nesnemizi oluşturuyoruz.*/
    stbDurum.Text=htTeknikSozluk.Count.ToString();
/* HashTable'ımızdaki eleman sayısını Count özelliği ile öğreniyoruz.*/
} 

private void btnEkle_Click(object sender, System.EventArgs e)
{

    try
    {
        htTeknikSozluk.Add(txtKey.Text,txtValue.Text);
        /* HashTable'ımıza key-value çifti ekleyebilmek için Add metodu kullanılıyor.*/
        lstAnahtar.Items.Add(txtKey.Text);
        stbDurum.Text=htTeknikSozluk.Count.ToString();  
    }
    catch(System.ArgumentException) /* Eğer var olan bir key'i tekrar eklemeye çalışırsak bu durumda ArgumentException istisnası fırlatılacaktır. Bu durumda, belirtilen key-value çifti HashTable koleksiyonuna eklenmez. Bu durumu kullanıcıya bildiriyoruz.*/
    {
        stbDurum.Text=txtKey.Text+" Zaten HashTable Koleksiyonunda Mevcut!";
    }
} 

private void lstAnahtar_DoubleClick(object sender, System.EventArgs e)
{
    string deger;
    deger=htTeknikSozluk[lstAnahtar.SelectedItem.ToString()].ToString();

    /* HashTable'daki bir değere ulaşmak için, köşeli parantezler arasında aranacak key değerini giriyoruz. Sonucu bir string değişkenine aktarıyoruz.*/
    MessageBox.Show(deger,lstAnahtar.SelectedItem.ToString());
} 

private void btnSil_Click(object sender, System.EventArgs e)
{
    if(htTeknikSozluk.Count==0)
    {
        stbDurum.Text="Çıkartılabilecek hiç bir eleman yok";
    }
    else if(lstAnahtar.SelectedIndex==-1)
    {
        stbDurum.Text="Listeden bir eleman seçmelisiniz";
    }    
    else
    {
        htTeknikSozluk.Remove(lstAnahtar.SelectedItem.ToString());
        /* Bir HashTable'dan bir nesneyi çıkartmak için, Remove metodu kullanılır. Bu metod parametre olarak çıkartılmak istenen değer çiftinin key değerini alır.*/
        lstAnahtar.Items.Remove(lstAnahtar.SelectedItem);
        stbDurum.Text="Çıkartıldı";
        stbDurum.Text=htTeknikSozluk.Count.ToString();
    }
} 

private void btnTumu_Click(object sender, System.EventArgs e)
{
    lstTumListe.Items.Clear(); 
    /* Aşağıdaki satırlarda, bir HashTable koleksiyonu içinde yer alan tüm elemanlara nasıl erişildiğini görmekteyiz. Keys metodu ile HashTable koleksiyonumuzda yer alan tüm anahtar değerlerini (key'leri), ICollection arayüzü(interface) türünden bir nesneye atıyoruz. Foreach döngümüz ile bu nesne içindeki her bir anahtarı, HashTable koleksiyonunda bulabiliyoruz.*/
    ICollection anahtar=htTeknikSozluk.Keys; 
    foreach(string a in anahtar)
    {
        lstTumListe.Items.Add(a+"="+htTeknikSozluk[a].ToString());
    } 
} 
```

Şimdi uygulamamızı çalıştırıp deneyelim.

![mk22_2.gif](/assets/images/2003/mk22_2.gif)

2. Programın Çalışmasnının sonucu.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.