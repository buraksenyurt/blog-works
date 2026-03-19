---
title: "Thread'lerde Öncelik(Priority) Durumları"
pubDate: 2004-01-05 12:00:00
categories:
  - C#
tags:
  - C#
  - .NET
  - Threading
---

# Thread'lerde Öncelik(Priority) Durumları
Değerli Okurlarım, Merhabalar.

İş parçacıklarını işlediğimiz yazı dizimizin bu üçüncü makalesinde, iş parçacıklarının birbirlerine karşı öncelik durumlarını incelemeye çalışacağız. İş parçacıkları olarak tanımladığımız metodların çalışma şıralarını, sahip oldukları öneme göre değiştirmek durumunda kalabiliriz. Normal şartlar altında, oluşturduğumuz her bir iş parçacığı nesnesi aynı ve eşit önceliğe sahiptir. Bu öncelik değeri Normal olarak tanımlanmıştır. Bir iş parçacığının önceliğini değiştirmek istediğimizde, Priority özelliğinin değerini değiştiririz. Priority özelliğinin.NET Framework'teki tanımı aşağıdaki gibidir.

public ThreadPriority Priority {get; set;}

Özelliğimiz ThreadPriority numaralandırıcısı (enumerator) tipinden değerler almaktadır. Bu değerler aşağıdaki tabloda verilmiştir.

Öncelik Değeri

Highest

AboveNormal

Normal

BelowNormal

Lowest

Tablo 1. Öncelik (Priority) Değerleri

Programlarımızı yazarken, iş parçacıklarının çalışma şekli verilen öncelik değerlerine göre değişecektir. Elbette tahmin edeceğiniz gibi yüksek öncelik değerlerine sahip olan iş parçacıklarının işaret ettikleri metodlar diğerlerine göre daha sık aralıklarda çağırılacak, dolayısıyla düşük öncelikli iş parçacıklarının referans ettiği metodlar daha geç sonlanacaktır. Şimdi olayı daha iyi canlandırabilmek için aşağıdaki örneğimizi geliştirelim. Daha önceden söylediğimiz gibi, bir iş parçacığının Priority özelliğine her hangibir değer vermez isek, standart olarak Normal kabul edilir. Buda tüm iş parçacıklarının varsayılan olarak eşit önceliklere sahip olacakları anlamına gelmektedir. Şimdi aşağıdaki formumuzu oluşturalım. Uygulamamız iki iş parçacığına sahip. Bu parçacıkların işaret ettiği metodlardan birisi 1' den 1000' e kadar sayıp bu değerleri bir label kontrolüne yazıyor. Diğeri ise 1000' den 1' e kadar sayıp bu değerleri başka bir label kontrolüne yazıyor. Formumuzun görüntüsü aşağıdakine benzer olmalıdır.

![mk35_1.gif](images/mk35_1.gif)

Şekil 1. Form Tasarımımız.

Şimdide program kodlarımızı yazalım.

```csharp
/* Bu metod 1' den 1000' e kadar sayar ve değerleri lblSayac1 isimli label kontrolüne yazar.*/

public void Say1()
{
     for(int i=1;i<1000;++i)
     {
          lblSayac1.Text=i.ToString();
          lblSayac1.Refresh();

 /* Refresh metodu ile label kontrolünün görüntüsünü tazeleriz. Böylece herbir i değerinin label kontrolünde görülebilmesini sağlamış oluyoruz. */
          for(int j=1;j<90000000;++j)
         {
             j+=1;
          }         
     }
}

/* Bu metod 1000' den 1' e kadar sayar ve değerleri lblSayac2 isimli label kontrolüne yazar.*/
public void Say2()
{
for(int i=1000;i>=1;i--)
     {
          lblSayac2.Text=i.ToString();
          lblSayac2.Refresh();         
          for(int j=1;j<45000000;++j)
         {
             j+=1;
          }   
     }
} 

/* ThreadStart ve Thread nesnelerimizi tanımlıyoruz. */
ThreadStart ts1;
ThreadStart ts2;
Thread t1;
Thread t2;
private void btnBaslat1_Click(object sender, System.EventArgs e)
{
     /* Metodlarımızı ThreadStart nesneleri ile ilişkilendiriyoruz ve ThreadStart nesnelerimizi oluşturuyoruz.*/
     ts1=new ThreadStart(Say1);
     ts2=new ThreadStart(Say2);

     /* İş parçacıklarımızı, ilgili metodların temsil eden ThreadStart nesnelerimiz ile oluşturuyoruz.*/
     t1=new Thread(ts1);
     t2=new Thread(ts2);

     /* İş parçacıklarımızı çalıştırıyoruz.*/
     t1.Start();
     t2.Start();

     btnBaslat1.Enabled=false;
     btnIptal.Enabled=true;
}

private void btnIptal_Click(object sender, System.EventArgs e)
{
     /* İş parçacıklarımızı iptal ediyoruz. */
     t1.Abort();
     t2.Abort();

     btnBaslat1.Enabled=true;
     btnIptal.Enabled=false;
}

private void btnKapat_Click(object sender, System.EventArgs e)
{
     /* Uygulamayı kapatmak istediğimizde, çalışan iş parçacığı olup olmadığını kontrol ediyoruz. Bunun için iş parçacıklarının IsAlive özelliğinin değerlerine bakıyoruz. Nitekim kullanıcının, herhangibir iş parçacığı sonlanmadan uygulamayı kapatmasını istemiyoruz. Ya iptal etmeli yada sonlanmalarını beklemeli. İptal ettiğimizde yani Abort metodları çalıştırıldığında hatırlayacağınız gibi, iş parçacıklarının IsAlive değerleri false durumuna düşüyordu, yani iptal olmuş oluyorlardı.*/
     if((!t1.IsAlive) && (!t2.IsAlive))
     {
          Close();
     }
     else
     {
          MessageBox.Show("Hala kapatılamamış iş parçacıkları var. Lütfen bir süre sonra tekrar deneyin.");
     }
}
```

Uygulamamızda şu an için bir yenilik yok aslında. Nitekim iş parçacıklarımız için bir öncelik ayarlaması yapmadık. Çünkü size göstermek istediğim bir husus var. Bir iş parçacığı için herhangibir öncelik ayarı yapmadığımızda bu değer varsayılan olarak Normal dir. Dolayısıyla her iş parçacığı eşit önceliğe sahiptir. Şimdi örneğimizi çalıştıralım ve kafamıza göre bir yerde iptal edelim.

![mk35_2.gif](images/mk35_2.gif)

Şekil 2. Öncelik değeri Normal.

Ben 11 ye 984 değerinde işlemi iptal ettim. Tekrar iş parçacıklarını Başlat başlıklı butona tıklayıp çalıştırırsak ve yine aynı yerde işlemi iptal edersek, ya aynı sonucu alırız yada yakın değerleri elde ederiz. Nitekim programımızı çalıştırdığımızda arka planda çalışan işletim sistemine ait pek çok iş parçacığıda çalışma sonucunu etkiler. Ancak aşağı yukarı aynı veya yakın değerle ulaşırız. Oysa bu iş parçacıklarının öncelik değelerini değiştirdiğimizde sonuçların çok daha farklı olabilieceğini söyleyebiliriz. Bunu daha iyi anlayabilmek için örneğimizi geliştirelim ve iş parçacıklarının öncelik değerleri ile oynayalım. Formumuzu aşağıdaki gibi tasarlayalım.

![mk35_3.gif](images/mk35_3.gif)

Şekil 3. Formumuzun yeni tasarımı.

Artık iş parçacıklarını başlatmadan önce önceliklerini belirleyeceğiz ve sonuçlarını incelemeye çalışacağız. Kodlarımızı şu şekilde değiştirelim. Önemli olan kod satırlarımız, iş parçacıklarının Priority özelliklerinin değiştiği satırlardır.

```csharp
/* Bu metod 1' den 1000' e kadar sayar ve değerleri lblSayac1 isimli label kontrolüne yazar.*/

public void Say1()
{
     for(int i=1;i<1000;++i)
     {
          lblSayac1.Text=i.ToString();
          lblSayac1.Refresh();

/* Refresh metodu ile label kontrolünün görüntüsünü tazeleriz. Böylece herbir i değerinin label kontrolünde görülebilmesini sağlamış oluyoruz. */
          for(int j=1;j<90000000;++j)
          {
               j+=1;
          }
     }
}

/* Bu metod 1000' den 1' e kadar sayar ve değerleri lblSayac2 isimli label kontrolüne yazar.*/

public void Say2()
{
    for(int i=1000;i>=1;i--)
     {
          lblSayac2.Text=i.ToString();
          lblSayac2.Refresh();
          for(int j=1;j<45000000;++j)
          {
               j+=1;
          }
     }
}
 
ThreadPriority tp1;

/* Priority öncelikleri ThreadPriority tipindedirler. */
ThreadPriority tp2;

/* OncelikBelirle metodu, kullanıcının TrackBar'da seçtiği değerleri göz önüne alarak, iş parçacıklarının Priority özelliklerini belirlemektedir. */

public void OncelikBelirle()
{
     /* Switch ifadelerinde, TrackBar kontrollünün değerine göre , ThreadPriority değerleri belirleniyor. */
     switch(tbOncelik1.Value)
     {
          case 1:
          {
               tp1=ThreadPriority.Lowest; /* En düşük öncelik değeri. */
               break;
          }
          case 2:
          {
               tp1=ThreadPriority.BelowNormal; /* Normalin biraz altı. */
               break;
          }
          case 3:
          {
               tp1=ThreadPriority.Normal; /* Normal öncelik değeri. Varsayılan değer budur.*/
               break;
          }
          case 4:
          {
               tp1=ThreadPriority.AboveNormal; /* Normalin biraz üstü öncelik değeri. */
               break;
          }
          case 5:
          {
               tp1=ThreadPriority.Highest; /* En üst düzey öncelik değeri. */
               break;
          }
     }
     switch(tbOncelik2.Value)
     {
          case 1:
          {
               tp2=ThreadPriority.Lowest;
               break;
          }
          case 2:
          {
               tp2=ThreadPriority.BelowNormal;
               break;
          }
          case 3:
          {
               tp2=ThreadPriority.Normal;
               break;
          }
          case 4:
          {
               tp2=ThreadPriority.AboveNormal;
               break;
          }
          case 5:
          {
               tp2=ThreadPriority.Highest;
               break;
          }
         }
         /* İş Parçacıklarımıza öncelik değerleri aktarılıyor.*/
         t1.Priority=tp1;
         t2.Priority=tp2;
}
/* ThreadStart ve Thread nesnelerimizi tanımlıyoruz. */

ThreadStart ts1;
ThreadStart ts2;

Thread t1;
Thread t2;

private void btnBaslat1_Click(object sender, System.EventArgs e)
{
         /* Metodlarımızı ThreadStart nesneleri ile ilişkilendiriyoruz ve ThreadStart nesnelerimizi oluşturuyoruz.*/
         ts1=new ThreadStart(Say1);
         ts2=new ThreadStart(Say2);

         /* İş parçacıklarımızı, ilgili metodların temsil eden ThreadStart nesnelerimiz ile oluşturuyoruz.*/
         t1=new Thread(ts1);
         t2=new Thread(ts2);

         OncelikBelirle(); /* Öncelik ( Priority ) değerleri, iş parçacıkları Start metodu ile başlatılmadan önce belirlenmelidir. */

/* İş parçacıklarımızı çalıştırıyoruz.*/
         t1.Start();
         t2.Start();
         btnBaslat1.Enabled=false;
         btnIptal.Enabled=true;

         tbOncelik1.Enabled=false;
         tbOncelik2.Enabled=false;
}

private void btnIptal_Click(object sender, System.EventArgs e)
{
         /* İş parçacıklarımızı iptal ediyoruz. */

         t1.Abort();
         t2.Abort();
         btnBaslat1.Enabled=true;
         btnIptal.Enabled=false;

         tbOncelik1.Enabled=true;
         tbOncelik2.Enabled=true;
}

private void btnKapat_Click(object sender, System.EventArgs e)
{
	 /* Uygulamayı kapatmak istediğimizde, çalışan iş parçacığı olup olmadığını kontrol ediyoruz. Bunun için iş parçacıklarının IsAlive özelliğinin değerlerine bakıyoruz. Nitekim kullanıcının, herhangibir iş parçacığı sonlanmadan uygulamayı kapatmasını istemiyoruz. Ya iptal etmeli yada sonlanmalarını beklemeli. İptal ettiğimizde yani Abort metodları çalıştırıldığında hatırlayacağınız gibi, iş parçacıklarının IsAlive değerleri false durumuna düşüyordu, yani iptal olmuş oluyorlardı.*/

     if((!t1.IsAlive) && (!t2.IsAlive))
     {
          Close();
     } else
     {
          MessageBox.Show("Hala kapatılamamış iş parçacıkları var. Lütfen bir süre sonra tekrar deneyin.");
     }
}
```

Şimdi örneğimizi çalıştıralım ve birinci iş parçacığımız için en yüksek öncelik değerini (Highest) ikinci iş parçacığımız içinde en düşük öncelik değerini (Lowest) seçelim. Sonuçlar aşağıdakine benzer olucaktır.

![mk35_4.gif](images/mk35_4.gif)

Şekil 4. Önceliklerin etkisi.

Görüldüğü gibi öncelikler iş parçacıklarının çalışmasını oldukça etkilemektedir. Geldik bir makalemizin daha sonuna. Bir sonraki makalemizde iş parçacıkları hakkında ilerlemeye devam edeceğiz. Görüşmek dileğiyle hepinize mutlu günler dilerim.
