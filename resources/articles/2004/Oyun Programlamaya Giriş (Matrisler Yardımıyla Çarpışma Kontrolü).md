---
title: "Oyun Programlamaya Giriş (Matrisler Yardımıyla Çarpışma Kontrolü)"
pubDate: 2004-12-04 12:00:00
categories:
  - C#
tags:
  - C#
---

# Oyun Programlamaya Giriş (Matrisler Yardımıyla Çarpışma Kontrolü)
Değerli Okurlarım Merhabalar,

Hafta sonu evde bilgisayarım başında internette gezinirken, tarihi oyunların anlatıldığı bir site ile karşılaştım. Aslında zaten eski oyunları araştırıyordum. Amacım bu oyunlara, oyun oynamak isteyen bir çocuk gözü ile değil, onların yapılarını ve çekirdeklerini anlamaya çalışacak bir yazılımcı gözüyle bakabilmekti. Sonuçta, içimdeki çocuk ağır basıp bir kaç tanesini saatlerce oynadım. Aralarında en çok hoşuma gidenlerden birisi PackMan'di. Packman, doğrusal düzlemde 4 yöne hareket edebilen bir kahramandı. Yolda kendisini rastgele konumlardan gelerek yakalamaya çalışan böceklerden kaçıyor ve bulduğu meyveleri yiyerekte puanlar topluyordu. Tam oyunu bitirmeme az kalmıştıki hiç beklenmedik bir şekilde böceklerden birisi tarafından yendim. Aslında ekrana bir süre donuk gözler ile bakmıştım. Nitekim, oyunu oynarken aklıma geçen gün okuduğum Oyun Programlama kitabı gelmişti.

Kitabın bir bölümünde, ekranda yer alan aynı boyutlu nesnelerin çarpışmalarının kontrolünde iki boyutlu bir matris dizisinden faydalanılıyordu. O anda, çarpışma tekniklerinin farklı bir teoremini Packman'a benzeyecek bir oyun ile inceleyebileceğimi düşündüm. Her ne kadar amacım tümüyle bir oyunu yazmak olmasada en azından Packman'imi ekrandaki duvarların içinden geçirmeyecek şekilde hareket ettirmek istiyordum.

Teori gayet basitti. İlk olarak oyun sahasını, eşit boyutlu karelere bölecektim. Bu karelerin boyutları oyundaki nesnelerin çevresini saran hayali karelerinki ile aynı olacaktı. Böylece bir kare içinde her zaman tek bir oyun elemanı tam olarak sığmış bulunacaktı. Daha sonra ekrandaki elemanları iki boyutlu bir matris dizisi içinde bir şekilde konumlandıracaktım. Son olarak, kahramanın her hareketinde yöne bağlı olaraktan önceki veya sonraki kare alanlarını kontrol edecek ve orada bir Duvar nesnesi var ise çarpışma olduğunu belirterek o yöne olan hareketi kesecektim.

![dikkat.gif](images/dikkat.gif)
Teoremin kilit noktası, oyun alanındaki karelerde bulunan elemanları, iki boyutlu bir Matris dizisi içerisinde temsil edebilmekti.

İlk olarak aşağıdaki gibi bir oyun alanını düşündüm.

![mk109_1.gif](images/mk109_1.gif)

Şekil 1. Oyun Sahası.

Öncelikle, Duvar, Muz ve kahramanımız Packo'ya ait imajları tasvir ettim. Bunların her birisi 20 piksel X 20 Piksel boyutlarındaki bir karenin iç kenarlarına teğer olacak büyüklükteydiler. Daha sonra, oyun sahamı 20 Piksel X 20 Piksel'lik kareler ile doldurdum. Dolayısıyla artık elimde, 20' ye 20' lik bir Matris vardı. Bu Matris yardımıyla ekrandaki her bir elemanın konumunu bilebilirdim. Tek yapmam gerken Matrisin ilgili elemanına, orada duran nesneyi temsil edecek sayısal bir değer vermekti. Örneğin şu anki haliyle, Packo'nun Matris'deki konumunu aşağıdaki gibi ifade edebilirdim.

Matris[5,8]=1;

Bu durumda, Packo'nun sağa doğru olan hareketinde herhangibir duvara çarpıp çarpmadığını kontrol etmek için, Y değerinin 1 fazlasına bakmak yeterli olacaktı.

Yani;

```csharp
eğer Matris[5,8+1]=2 ise (ki Duvarlarıda 2 sayısı ile ifade edebilirdim.)
     Çarpışma var, Sağa gitme.
eğer Matris[5,8+1]=3 ise (ki Muzları 3 ile ifade edebilirdim.)
     Sağa git, Matris[5,9] daki Muzu ekrandan sil, Puanı arttır.
eğer Matris[5,8+1]=0 ise (ne duvar ne de Muz var ise.)
     Çarpışma yoktur. Yola devam et.
```

Bu kontrolü Packo'nun yapacağı doğrusal her hareket için uygulayabilirdim. Böylece,

![dikkat.gif](images/dikkat.gif)
Hareket yönüne göre bir sonraki adımda yer alan elemanları Matris dizisi içinde bularak çarpışma kontrolünü gerçekleştirebilirdim.

Teoremi kafamda pekiştirdikten sonra, sıra bunu uygulamaya dökmeye gelmişti. Elbetteki bir Windows uygulaması için böyle bir teoremi araştırmaya çalışırken bir takım zorluklar ile karşılaşabilirdim. Örneğin, oyun başladığında Duvarların, Muzların ve Packonun rastgele ekrana konumlandırılması. Ekranda piksel bazında tutulan karesel alanların, Matris dizisi içerisinde nasıl indislendirilebileceği. Öyle ya, 400 piksel'e 400 piksel'lik bir Form alanını, 400*400 elemanlı bir Matris dizisinde aynen uygulamak gereksiz yere hafıza tüketimine neden olurdu. Ya da, ekrandaki bir Muz'un üstünden geçildiğinde o resmin nasıl kaldırılacağı. Bu gibi pek çok sorunu önceden düşünmek ve uygulamayı ona göre planlamak gerekiyordu. Bu amaçla önce düşündüm ve sonra aşağıdaki kodları geliştirdim.

Konumlandırma işlemleri için kullandığım sınıf,

```csharp
using System;

namespace Packo
{
    /* Bu sinif yardimiyla oyun alaninda kullandigimiz nesnelerin X ve Y koordinatlari ve duvar sayilari için rastgele degerler ürettiriyoruz. Ekranimizi 20' ye 20'e lik karelere ayirdigimiz için random sinifinin Next metodunu buna uygun sekilde çagiriyoruz. */
    public class Konumlandir
    {
        private int X,Y,duvarSayisi;
        System.Random r;

        public Konumlandir()
        {
            r=new Random(); 
        }
    
        /* X koordinatlari için (herhangibir oyun elemaninin Left özelliginin degeri için) bir özellik tanimliyoruz. Bu özellik Read-Only formatindadir. */
        public int YerlestirX
        {
            get
            {
                X=r.Next(1,20)*20; /* Oyun alani 400 piksele 400 piksel boyutunda oldugu için, 1 ile 20 arasindaki rakami 20 kat sayisi ile çarpiyoruz.*/
                return X;
            }
        }

        /* Y koordinatlari için (herhangibir oyun elemaninin Top özelliginin degeri için) bir özellik tanimliyoruz. Bu özellik Read-Only formatindadir. */
        public int YerlestirY
        {
            get
            {
                Y=r.Next(1,20)*20;
                return Y;
            }
        }
        /* Ekrana 10 ile 30 arasinda restgele bir degerde Duvarlar koymak için bu özelligi kullaniyoruz. Bu özellik Read-Only formatindadir. */
        public int DuvarSayisi
        {
            get
            {
                duvarSayisi=r.Next(10,30);
                return duvarSayisi;
            }
        }
    }
}
```

Ana Program;

```csharp
Konumlandir k;

/* Ekrani 20*20 lik bir matris ile ele alacagiz. */
int[,] Matris=new int[20,20];

/* Matristeki her bir elemanin hangi oyun nesnesini (duvar,kahramanimiz packocuk ve Muz) temsil ettigini daha kolay kontrol edebilmek için sayisal degerleri bir enum sabiti ile anlamlandiriyoruz.*/
enum AlanSahibi
{
    Pakocuk=1,
    Duvar=2,
    Muz=3
}

/* Ekrana duvar elemani, rastgele koordinatlara gelecek sekilde ekleniyor.*/
private void DuvarEkle()
{ 
    /*Bir PictureBox nesnesi tanimlaniyor.*/
    PictureBox pb=new PictureBox();
    /*Nesnemizin içerecegi resim yükleniyor. */
    pb.Image=System.Drawing.Image.FromFile("duvar.jpg");
    /*Nesnemizin ekrandaki yerlesimi için gerekli koordinat ayarlamalari yapiliyor.*/
    pb.Top=k.YerlestirY;
    pb.Left=k.YerlestirX;
    /* Duvarin ekrandaki piksel bazli koordinatlarini Matrisimizdeki elemanlar ile uyusturabilmek için 20 ile bölüyoruz.     Matrisin bu elemanina Duvar enum sabitinin degerinin veriyoruz.*/
    Matris[pb.Left/20,pb.Top/20]=(int)AlanSahibi.Duvar;
    /* Nesnemizin boyutlari belirleniyor.*/
    pb.Width=20;
    pb.Height=20;
    pb.SizeMode=PictureBoxSizeMode.StretchImage;
    /* Nesnemiz formumuzun Controls koleksiyonuna ekleniyor.*/
    this.Controls.Add(pb);
}

/* Ekrana bir Muz elemani, rastgele koordinatlara gelecek sekilde yerlestiriliyor.*/
private void MuzEkle()
{ 
    /*Muz nesnesinin üzerinden geçildiğinde onu ekrandan kaldırabilmek için kontrolün adını bilmem gerekiyor.Bunun için MuzEkle metodunun adını bulunduğu koordinata göre tanımlıyoruz.*/
    string ad; 
    PictureBox pb=new PictureBox(); 
    pb.Image=System.Drawing.Image.FromFile("muz.jpg");
    pb.Top=k.YerlestirY;
    pb.Left=k.YerlestirX;
    Matris[pb.Left/20,pb.Top/20]=(int)AlanSahibi.Muz;
    ad="MUZ_"+Convert.ToString((pb.Left/20))+"_"+Convert.ToString((pb.Top/20));
    pb.Name=ad;
    pb.Width=20;
    pb.Height=20;
    pb.SizeMode=PictureBoxSizeMode.StretchImage;
    this.Controls.Add(pb); 
}

/* Kahramanimiz Packo' nun ekrandaki konumu, Matris dizisindeki yeri ve saga veya sola bakacagi resmi belirleniyor. Ayrica, ekrana DuvarSayisi kadar Duvar ve Muz elemanlari ekleniyor.*/
private void Baslat()
{
    /* Rastegele X,Y ve duvar sayilari için kullandigimi Konumlandir sinifina ait bir nesne örnegi olusturuluyor.*/
    k=new Konumlandir();
    resPacko.Left=k.YerlestirX;
    resPacko.Top=k.YerlestirY;
    Matris[resPacko.Left/20,resPacko.Top/20]=(int)AlanSahibi.Pakocuk;
    /*Egere resPacko ekranin sol yarim küresinde ise, Sola bakan resmi gösteriliyor.*/
    if(resPacko.Left<200)
        resPacko.Image=System.Drawing.Image.FromFile("packoSol.jpg");
    /*Egere resPacko ekranin sag yarim küresinde ise, Saga bakan resmi gösteriliyor.*/
    if(resPacko.Left>200)
        resPacko.Image=System.Drawing.Image.FromFile("packoSag.jpg");
    resPacko.Visible=true;
    for(int i=1;i<k.DuvarSayisi;i++)
    {
        DuvarEkle();
        MuzEkle();
    }
}

int carpismaDurumu=0;

/* Eğer üstünden geçtiğimiz nesne Muz ise onu ekrandan kaldırıyoruz. Bunu yaparken Form üzerindeki kontrollerde gezinip, kontrolün adını buluyor ve Remove metodunu çağırıyoruz.*/
private void MuzYokEt(int X,int Y)
{ 
    /*Önce Matirsimizin X,Y elemanının Muz olup olmadığına bakıyoruz.*/
    if(Matris[X,Y]==(int)AlanSahibi.Muz)
    {
        /*Eğer Muz ise dizinin bu elemanının değerini 0 yapıyoruz. Böylece Muz nesnemizi diziden çıkarmış oluyoruz.*/
        Matris[X,Y]=0;
        /* Daha sonra PictureBox' ımızın adını tedarik ediyoruz. */
        string muzX=X.ToString();
        string muzY=Y.ToString();
        string KontrolAdi="MUZ_"+muzX+"_"+muzY;
        /*Döngü ile, Form içindeki tüm kontroller arasında geziniyoruz.*/
        for(int i=0;i<this.Controls.Count;i++)
        {
            /*Eğer güncel kontrolün adı, bizim Muz nesnemizinki ile aynı ise, bu PictureBox' ı Formumuzdan çıkartıyoruz.*/
            if(this.Controls[i].Name.ToString()==KontrolAdi)
            {
                this.Controls.Remove(this.Controls[i]);
            }    
        }
    }
}

/* Çarpisma kontrolünün yapildigi metod. Bu metod Packo' nun X, Y koordinatlari ile hareket ettigi yönü (saga,sola,yukariya,asagiya) parametre olarak aliyor. Aldigi X,Y koordinatlari ve yöne göre, bir sonraki kare alaninda bir Duvar nesnesi olup olmadigina bakiyor.*/ 
private void CarpismaKontrol(int X,int Y,char Yon)
{
    /*Eger sola hareket ediyorsak ve Matrisimizin [X indisinin 1 önceki elemani,Y] AlanSahibi.Duvar enum sabitinin degerine esit ise çarpisma vardir. */
    if(Yon=='A')
    {
        int alanSahibi=Matris[X-1,Y];
        if(alanSahibi==(int)AlanSahibi.Duvar)
            carpismaDurumu=1;
        else
            carpismaDurumu=0;
        MuzYokEt(X,Y);
    }
    /*Eger saga hareket ediyorsak ve Matrisimizin [X indisinin 1 sonraki elemani,Y] AlanSahibi.Duvar enum sabitinin degerine esit ise çarpisma vardir. */
    if(Yon=='D')
    {
        int alanSahibi=Matris[X+1,Y];
        if(alanSahibi==(int)AlanSahibi.Duvar)
            carpismaDurumu=1;
        else
            carpismaDurumu=0;
        MuzYokEt(X,Y);
    }
    /*Eger asagi hareket ediyorsak ve Matrisimizin [X,Y indisinin 1 sonraki elemani] AlanSahibi.Duvar enum sabitinin degerine esit ise çarpisma vardir. */
    if(Yon=='S')
    {
        int alanSahibi=Matris[X,Y+1];
        if(alanSahibi==(int)AlanSahibi.Duvar)
            carpismaDurumu=1;
        else
            carpismaDurumu=0;
        MuzYokEt(X,Y);
    }
/*Eger yukari hareket ediyorsak ve Matrisimizin [X,Y indisinin 1 önceki elemani] AlanSahibi.Duvar enum sabitinin degerine esit ise çarpisma vardir. */
    if(Yon=='W')
    {
        int alanSahibi=Matris[X,Y-1];
        if(alanSahibi==(int)AlanSahibi.Duvar)
            carpismaDurumu=1;
        else
            carpismaDurumu=0;
        MuzYokEt(X,Y);
    }
}

/*Form üzerinde A (sol), S (asagi), D (saga), W (yukari) tuslarina basildikça çarpisma kontrolü yapiliyor. Eger çarpisma var ise, belirtilen yöndeki dogrusal harekete (20 piksellik öteleme) izin verilmiyor. Aksi halde harekete devam ediliyor.*/
private void frmPacko_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
{
    int x=resPacko.Left/20;
    int y=resPacko.Top/20;

    if(e.KeyChar==(Char)Keys.A)
    {
        resPacko.Image=System.Drawing.Image.FromFile("packoSol.jpg");
        CarpismaKontrol(x,y,'A');
        if(carpismaDurumu==1)
            MessageBox.Show("DUVARA ÇARPTIN");
        else
            resPacko.Left-=20;
    }
    if(e.KeyChar==(Char)Keys.D)
    {
        resPacko.Image=System.Drawing.Image.FromFile("packoSag.jpg");
        CarpismaKontrol(x,y,'D');
        if(carpismaDurumu==1)
            MessageBox.Show("DUVARA ÇARPTIN");
        else
            resPacko.Left+=20;
    } 
    if(e.KeyChar==(Char)Keys.S)
    {
        resPacko.Image=System.Drawing.Image.FromFile("packoAsagi.jpg");
        CarpismaKontrol(x,y,'S');
        if(carpismaDurumu==1)
            MessageBox.Show("DUVARA ÇARPTIN");
        else
            resPacko.Top+=20;
    }
    if(e.KeyChar==(Char)Keys.W)
    {
        resPacko.Image=System.Drawing.Image.FromFile("packoYukari.jpg");
        CarpismaKontrol(x,y,'W');
        if(carpismaDurumu==1)
            MessageBox.Show("DUVARA ÇARPTIN");
        else
            resPacko.Top-=20;
    }
} 

/*Oyunu kapatmak ve baslatmak kullandigimiz menüleri ContextMenu içerisinde kullaniyoruz.*/
private void menuItem3_Click(object sender, System.EventArgs e)
{
    Close();
}

private void menuItem1_Click(object sender, System.EventArgs e)
{
    Baslat();
}

/* Kontrol amaciyla, Matris dizisinin degerlerini Text tabanli bir dosyaya da yazabiliyoruz.*/
private void menuItem4_Click(object sender, System.EventArgs e)
{
    System.IO.FileStream fs=new System.IO.FileStream("kontrol.txt",System.IO.FileMode.OpenOrCreate,System.IO.FileAccess.Write);
    System.IO.StreamWriter sw=new System.IO.StreamWriter(fs);
    for(int i=0;i<20;i++)
    {
        for(int j=0;j<20;j++)
        {
            sw.Write("{0,2},{1,2}={2,3}",i,j,Matris[i,j]);
        }
        sw.WriteLine();
    }
    sw.Flush();
    sw.Close();
    fs.Close();
}
```

Kodlar her ne kadar uzun görünsede programın tek yaptığı, Packo'yu duvarların içinden geçirmeden hareket ettirmek ve yolda gördüğü Muz'ları toplamasını sağlamak. Örneğin, uygulamayı çalıştırdığımızda bir duvara hangi yönden gelirsek gelelim, Matris dizimiz içinde Packo'dan sonraki elemanlar kontrol edilecek ve duvara çarpılıp çarpılmadığına bakılacaktır.

![mk109_2.gif](images/mk109_2.gif)

Şekil 2. Duvara Çarpış.

Diğer yandan, eğer Packo bir Muz üzerinden geçerse, bu Muz nesnesini temsil eden PictureBox Form üzerinden kaldırılacak, aynı zamanda Matris dizimizdeki ilgili Muz elemanın değeride sıfırlanacaktır.

![mk109_3.gif](images/mk109_3.gif)

Şekil 3. Packo Muzları Yiyebiliyor.

Görüldüğü gibi, Matris tekniği ile eşit karelere bölünmüş sahalardaki nesnelerin birbirleri ile olan çarpışmalarını kontrol edebilmek son derece basit. Bu minik program elbetteki başlangıç aşamasında. Örneğin, her muz yiyişinden sonra puanlama sistemi olması, duvarların seri olacak şekilde ekrana dizilmesi, Packo dışında hareket eden ve Packo'yu yemeye çalışan böceklerin farklı hareketleri vs... Bu kısımların geliştirilmesinide siz değerli okurlarıma bırakıyorum. Uygulamaya kaldığı yerden devam edebilirsiniz. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.
