---
layout: post
title: "StreamReader Sınıfı Yardımıyla Dosya Okumak"
date: 2004-01-01 12:00:00 +0300
categories:
  - csharp
tags:
  - ado.net
  - csharp
  - .net
  - stream
---
Bugünkü makalemizde, sistemimizde yer alan text tabanlı dosyaları nasıl okuyabileceğimizi incelemeye çalışacağız..NET ortamında, dosyaların okunması için streamler (akımlar) kullanılır. Bugün işleyeceğimi StreamReader sınıfıda bunlardanbir tanesidir. StreamReader sınıfı dosyaların okunmasını, dosyalara yazılmasını vb.. sağlar. StreamReader sınıfını bir FileStream nesnesi ile kullanabileceğimiz gibi, tek başınada kullanabiliriz. Kullanabileceğimiz yapıcı metodlardan birisi;

```csharp
public StreamReader( Stream stream );
```

dir. Bu yapıcı metod Stream tipinden bir nesne alır. Bu stream nesnesi çoğunlukla FileStream sınıfından türetilmiş bir nesne olur. Bu yapıcı metodumuz dışında direkt olarak okuma amacı ile StreamReader nesnesini;

```csharp
public StreamReader( string path );
```

yapıcısı ilede oluşturabiliriz. Burada string tipindeki path değişkenimiz, okumak amacıyla açacağımız dosyanın tam adresini temsil etmektedir. StreamReader nesnesi ile dosyamızı açtıktan sonra dosya içindeki verileri ReadLine metodu ile okuyabiliriz. ReadLine metodu, dosyadan her defasında bir satır okur ve bunun string olarak geriye döndürür. Metodun prototipi aşağıdaki gibidir.

```csharp
public override string ReadLine();
```

Genellikle bu metod bir while döngüsü ile kullanılır. Bu sayade dosyanın tüm içeriğinin okunması sağlanmış olur. ReadLine metodu geriye null değerini döndürdüğü zaman dosyanın sonuna gelindiği anlaşılır. Bu nedenle While döngüsünde kontrol ifadesi okunan her bir satırın null olup olmadığına bakar.

Şimdi bu kısa açıklamaların ardırdan dilerseniz uygulamamızı yazalım. Bu küçük uygulamamızda kullanıcının seçmiş olduğu bir dosyayı bir listBox kontrolüne satır bazında açıcağız.Öncelikle aşağıdaki örnek formu oluşturalım. OpenFileDialog kontrolümüzün Filter özelliğinede ekranda görülen değerleri aktarıyoruz. Böylece OpenFileDialog kontrolümüz açıldığında cs,vb uzantılı dosyaları ve tüm dosyaları görebileceğiz.

![mk32_1.gif](/assets/images/2004/mk32_1.gif)

Şekil 1. Form Tasarımımız.

Şimdi dilersenin projemizin kodlarını yazalım.

```csharp
private void btnDosyaAc_Click(object sender, System.EventArgs e)
{
    if (ofdDosya.ShowDialog() == DialogResult.OK)
    {
        this.Text = ofdDosya.FileName.ToString();
        FileStream d = new FileStream(ofdDosya.FileName, FileMode.Open, FileAccess.Read); /* Burada okuma amacı ile OpenFileDialog kontrolünden seçtiğimiz dosyaya bir akım oluşturyoruz. */
    }
    StreamReader sr = new StreamReader(d); /* Şimdi ise StreamReader nesnemizi FileStream nesnesini kullanarak oluşturuyoruz. */
    String input;
    /* Bu döngü, sr isimli StreamReader nesnemizin temsil ettiği akım vasıtasıyla, dosyamızdan bir satır alır ve bunu bellekteki tampon bölgeye yerleştirir. 
     * Daha sonra bunu bir string değişkene atıyoruz. Ardından bunun değerinin null olup olmadığına bakıyoruz. Null olması halinde dosya sonuna geldiğimiz anlaşılmaktadır. 
     * ReadLine metodu otomatik olarak bir satırı tampona aldıktan sonra, akımdan bir sonraki satırı okur ve belleğe alır. */
    while ((input = sr.ReadLine()) != null)
    {
        lstDosya.Items.Add(input);
        /* ListBox kontrolümüze ounan her bir satırı ekliyoruz.*/
    }
    sr.Close();
    /* Son olarak StreamReader nesnemizi ve FileStream nesnemizi kapatıyoruz. */
    d.Close();
}
```

Şimdi uygulamamızı çalıştıralım. Görüldüğü gibi OpenFileDilaog kutumuzda Filter özelliğinde belirlediğimiz dosyalar görünüyor.

![mk32_2.jpg](/assets/images/2004/mk32_2.jpg)

Şekil 2. OpenFileDialog İletişim Kutumuz.

![mk32_3.gif](/assets/images/2004/mk32_3.gif)

Şekil 3. Dosyamızın içeriği.

Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.