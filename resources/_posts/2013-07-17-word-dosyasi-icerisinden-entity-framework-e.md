---
layout: post
title: "Word Dosyası İçerisinden Entity Framework’ e"
date: 2013-07-17 17:26:00 +0300
categories:
  - office-development
tags:
  - office-development
  - csharp
  - dotnet
  - aspnet
  - aspnet-mvc
  - ado-net
  - entity-framework
  - wcf
  - windows-forms
  - http
  - authentication
  - generics
  - visual-studio
---
Geliştirdiğimiz veya kullanmakta olduğumuz yazılım ürünlerine dahil olan, farklı segmentlerden gelen pek çok kullanıcı profili vardır. Farklı profillerin olması, bazı hallerde geliştirilmekte olan ürünlerin başarısını doğrudan etkilemektedir. Bir fotoğraf işleme programını geliştirirken çoğu zaman annemizin olası kullanıcı profilleri arasına gireceğini pek düşünmeyiz. Genellikle fotoğraf işleme programını kullanacak olanların, en azından temek düzeyde fotoğrafçılık bilgisine sahip olduğunu kabul eder, menü komutlarını buna göre belirler, arayüzü buna göre hazırlarız. Ama bazı uygulamalarda annemizi hedef alır ve çektiği fotoğraflara kolayca efekt uygulamasına bir kaç basit adımda olanak tanır. Örneğin instagram’ ın iPhone uygulamasında olduğu gibi.

[![275344](/assets/images/2013/275344_thumb.jpg)](/assets/images/2013/275344.jpg)

Bir yazılım geliştirme ürününü tasarlarken ise, son kullanıcının programcılar olduğunu varsayar ve arabirimin karmaşık olmasının herhangibir sorun oluşturmayacağını düşünürüz. Oysaki Team Foundation Server gibi geniş ürün yelpazesine sahip aileler düşünüldüğünde, işe dahil olan farklı profildeki kullanıcılar için işleri kolaylaştırıcı şekilde düşünüldüğüne şahit oluruz.

Söz gelimi proje yöneticisinin, Ms Project ürünü ile TFS’ e entegre olabildiğini, Scrum Master’ ın isterse tüm Product Backlog içeriğini bir Excel dosyasına indirip senkronize edebildiğini, Yazılım Mimarının ve Geliştiricilerin, Visual Studio ile ortama bağlanabildikleri ama birim müdürü için sadece Team Explorer’ ın kafi gelebildiğini, İş Analisti gibi geliştiriciden farklı profile sahip elemanların ise Web arayüzünü kullanarak basit bir şekilde Requirement ekleyebildikleri görürüz.

Sonuç olarak geliştirilen ürünün kullanım alanına dahil olan tüm personeli göz önüne alarak hareket etmemiz gerektiğinin farkında olmalıyız. İşte bu yazımızda bu konuyu tecrübe etmeye çalışacağımız bir örnek geliştiriyor olacağız. Şimdi izleyen paragraftaki senaryoyu göz önüne alalım.

Senaryo

Selim Usta bir oto yedek parçacıdır. İşlerini kolaylaştırmak amacıyla uzun zamandır bilgisayar öğrenmeye çalışmaktadır ve en nihayetinde Word, Excel gibi ofis uygulamalarını basit seviyede de olsa kullanmaya başlamıştır. En sık yaptığı işlemlerden birisi ise stoğuna dahil ettiği yeni ürünleri bilgisayarda bir Word dosyasına kaydetmektir. Lakin zaman içerisinde Word dosyası epeyce şişmiş, içeride bir şey aramak epeyce zorlaşmıştır. Üstelik içeriğin düzgün bir formatı da yoktur.

Aslında ihtiyacı olan basit bir arayüz ile stoğunu takip etmektir. Rukiye, Selim Usta’ nın torunudur ve Üniversite son sınıftadır. Matematik Mühendisliği okumaktadır. Dedesinin işlerini kolaylaştırmak için bir Web arayüzü hazırlamıştır. Asp.Net MVC kullanmıştır. İyi renkler seçmiş, adımları sadeleştirmiş ve Selim Usta için kullanıcı deneyimi (User Experience) epeyce yüksek bir arabirim sunmuştur. Herşey yolunda gibidir. Lakin Selim Usta’ nın vazgeçemediği alışkanlıkları vardır

![Smile](/assets/images/2013/wlEmoticon-smile_87.png)

Bir gün torununa şöyle der; “Kızım keşke şu yeni gelen tamponları bu öğrendiğim Word ile de kayıt edebilsem. Bu internet üzerinden girmek zor geliyor bana…”

İşte şimdi bizim devreye girme sıramız. Bu yazımızda gerçekleştireceğimiz iş, bir Word dokümanının içeriğini basit bir şekilde bir veritabanına eklemek olacak. Bunun için Word üzerinde bazı Windows Form kontrollerini kullanıyor olacağız ve ayrıca kod yazarak, Entity Framework tabanlı giriş işlemleri gerçekleştireceğiz. Dilerseniz hiç vakit kaybetmeden yola koyulalım.

Word Document Projesini Oluşturmak

Visual Studio 2012 ortamımızda yeni bir Solution açarak işe başlayabiliriz. İlk projemiz Visual C#->Office/SharePoint/Office Add-ins sekmesinde yer alan Word 2010 Document tipinden olacak.

[![wrp_1](/assets/images/2013/wrp_1_thumb.png)](/assets/images/2013/wrp_1.png)

ProductDocument olarak projeyi isimlendirdikten sonra küçük bir soru ile karşılacağız.

[![wrp_2](/assets/images/2013/wrp_2_thumb.png)](/assets/images/2013/wrp_2.png)

Yeni bir doküman oluşturarak ilerlemeyi tercih edelim. Ama var olan bir dokümanı da kullanabiliriz. Bu işlemlerin sonucunda Visual Studio ortamımız aşağıdaki şekle bürünecektir

![Surprised smile](/assets/images/2013/wlEmoticon-surprisedsmile_5.png)

[![wrp_3](/assets/images/2013/wrp_3_thumb.png)](/assets/images/2013/wrp_3.png)

Dikkat edileceği üzere IDE’ nin göbeğinde docx uzantılı bir Word belgesi yer almaktadır. Hatta Solution Explorer penceresine baktığımızda, ThisDocument.cs isimli bir sınıf dosyası olduğunu, Toolbox içerisinde ise pek çok Form kontrolünün yer aldığını görürüz

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_184.png)

Bu, kısaca şu anlama gelmektedir; Word Object Model’ in üzerine Windows Form kontrollerini ekleyebilir ve C# ile kodlama yapabiliriz. Öyleyse Selim Usta için basit bir içerik hazırlayarak ilerleyelim (Word Object Model ile ilişkili olarak [bu adresten daha detaylı bilgi](http://msdn.microsoft.com/en-us/library/vstudio/kw65a0we.aspx) tedarik edebilirsiniz)

Word Tasarımını Yapalım

Word belgesini Ribon tarafında yer alan bileşenler ile donatabileceğimiz gibi Toolbox üzerinde yer alan Component’ leri içermesini de sağlayabiliriz. Bu anlamda melez bir arayüz geliştirme ortamı oluştuğunu ifade edebiliriz. Hem Word hem de Windows Forms kontrollerini bir arada ele alabilmekteyiz. Bu düşünceler ışığında Selim Usta için aşağıdaki formu oluşturduğumuzu düşünelim.

[![wrp_4](/assets/images/2013/wrp_4_thumb.png)](/assets/images/2013/wrp_4.png)

Dikkat edileceği üzere Word içeriğine TextBox, DateTimePicker, NumericUpDown, Button kontrolleri eklenmiştir. Kontrollere kod tarafı için anlamlı isimler vererek ilerleyelim. Ben örnekte [KontrolTipi][TabloKolonAdı] şeklinde bir notasyon kullandım. Yani Name alanı için TextBoxName, açıklama alanı için TextBoxDescription vb…

Entity Framework Çözümünün Eklenmesi

Solution içerisinde Entity Framework tabanlı bir Class Library projesi de yer almaktadır. Söz konusu proje bir Ado.Net Entity Model öğesi bulundurmakta olup aşağıdaki diagramda görülen içeriğe sahiptir.

[![wrp_5](/assets/images/2013/wrp_5_thumb.png)](/assets/images/2013/wrp_5.png)

Product Entity tipi, Depomuz isimli SQL 2008 veritabanında yer alan Product tablosunu işaret etmektedir. Otomatik artan ve Primary Key olan ProductId alanı dışında, Name (nvarchar (50)), Description (nvarchar (250)), ListPrice (decimal (18,0)), RealPrice (decimal (18,0)), InsertDate (Date), Quantity (int) ve Notes (nvarchar (250)) gibi kolonları da içermektedir.

Pek tabi bir Word dokümanından C# kodlarını kullanarak farklı bir kütüphaneye erişebildiğimize göre, WCF servislerini de çağırmamız mümkündür. Özellikle bir şirketin bu tip bir dokümanı kullanarak, çeşitli Repository’ lere kayıt atması istenen durumlarda, bir Servis kanalına uğranmasını sağlamak yerinde bir davranış olabilir. (Dolayısıyla senaryonuzu, Entity Framework kütüphanesi ile Word projesi arasına bir WCF Servisi sokarak genişletebilirsiniz)

Kodlar

Tabiki ilk etapta Word projesinin, Entity Library kütüphanesini referans etmesi gerekmektedir.

> Burada dikkat edilmesi gereken noktalardan birisi de Target Framework seçimidir. Oluşturduğumuz Word Document projesi.Net Framework 4.0 odaklıdır. Bu sebeple Entity Framework tabanlı olan Class Library projesinin de.Net Framework 4.0 odaklı olması gerekmektedir. Aksi durumda Target Framework uyuşmazlığı oluşuacak ve Word Document tipi projeye eklenen referans için bir Warning alınacaktır.

Kod yazımı işin eğlenceli taraflarından birisidir elbette

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_184.png)

Word projesindeki docx dosyası, bir de C# kod dosyası içermektedir. Burada Word’ ün Object Model’ ine this anahtar kelimesi üzerinden kolayca erişilebilinir. Daha da önemlisi bazı olaylar (Events) yüklenerek dokümanının davranışları fonksiyonel olarak değiştirilebilir. Örneğin bu Word dokümanı kayıt edilmeden önce veya sonra devreye girecek olay metodları (Event Handlers) dahil edilebilir. Dolayısıyla yapabileceklerimiz oldukça geniş bir yelpazeye yayılmaktadır. Biz örneğimizde oldukça basit ilerledik ve aşağıdaki kod yapısını oluşturduk.

```csharp
using AzonEntityLibrary; 
using System; 
using System.Windows.Forms; 
using Word = Microsoft.Office.Interop.Word;

namespace ProductDocument 
{ 
    public partial class ThisDocument 
    { 
        private void ThisDocument_Startup(object sender, System.EventArgs e) 
        { 
            this.Application.DocumentBeforeSave += Application_DocumentBeforeSave; 
        }

        private void ThisDocument_Shutdown(object sender, System.EventArgs e) 
        {

        }

        void Application_DocumentBeforeSave(Word.Document Doc, ref bool SaveAsUI, ref bool Cancel) 
        { 
            SaveToDatabase(); 
        }

        private bool SaveToDatabase() 
        { 
            bool result = false; 
            try 
            { 
                using (DepomuzEntities context = new DepomuzEntities()) 
                { 
                    Product newProduct = new Product 
                    { 
                        Name = TextBoxName.Text, 
                        Description = TextBoxDescription.Text, 
                        InsertDate = DateTimePickerEnterDate.Value, 
                        ListPrice = System.Convert.ToDecimal(TextBoxListPrice.Text), 
                        RealPrice = System.Convert.ToDecimal(TextBoxRealPrice.Text), 
                        Quantity = System.Convert.ToInt32(NumericUpDownQuantity.Value), 
                        Notes = TextBoxNotes.Text 
                   }; 
                    context.Products.Add(newProduct); 
                    context.SaveChanges();

                    result = true;

                    MessageBox.Show( 
                        string.Format("Selim Ustam ürün {0} numarası ile dosyalandı" 
                        ,newProduct.ProductId.ToString())); 
                } 
            } 
            catch(Exception excp) 
            { 
                MessageBox.Show("Selim Ustam beni arar mısın? Sanırım işlemin sırasında bir hata oluştu"); 
                //TODO@Rukiye burada oluşan istisnaları kendime mail atayım ve hatta log dosyasına yazdırayım 
            }

            return result; 
        }

        private void ButtonAdd_Click(object sender, EventArgs e) 
        { 
            SaveToDatabase(); 
        }

        #region VSTO Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor. 
        /// </summary> 
        private void InternalStartup() 
        { 
           this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click); 
            this.Startup += new System.EventHandler(this.ThisDocument_Startup); 
            this.Shutdown += new System.EventHandler(this.ThisDocument_Shutdown); 
        }

        #endregion 
    } 
}
```

Kod dosyasında dikkat çeken noktalardan birisi default olarak üretilen olay metodlarıdır. ThisDocumentStartup ve ThisDocumentShutdown. Bu olay metodları Word dokümanı başlatılırken ve kapatılırken devreye girmektedir. Örnekte, ThisDocumentStartup fonksiyonunda, DocumentBeforeSave olay metodunun yüklenmesi işlemi yapılmıştır.

Yeni bir ürünün Entity Framework üzerinden veritabanına kayıt edilmesi işlemi iki farklı noktadan tetiklenmektedir. Birincisi, Word dokümanının Save komutu verildikten ama Save operasyonu tamamlanmadan öncedir. Bunun için dikkat edileceği üzere Application referansı üzerinden DocumentBeforeSave olay metodu yüklenmiştir. İkinci tetikleme noktamız ise Word dokümanı üstüne atılmış olan Button kontrolünün Click olay metodudur. Yani, bir Word dokümanının var olan Object Model’ ine ait olay metodlarından yararlanılabileceği gibi, doküman üzerine bırakılmış Windows Form kontrollerinin olay metodlarından da yararlanılabilinir.

SaveToDatabase metodu içerisinde Context nesnesi kullanılmaktadır. Word dokümanı üzerindeki kontrollere ait bilgiler yeni üretilen Product nesne örneğinin özelliklerine set edildikten sonras ise kayıt altına alma işlemi yapılmaktadır. Yeni Product örneği, Products özelliği üzerinden Add metodu ile generic DbSet’ e ilave edildikten sonra SaveChanges çağrısı ile de veritabanına yazılmaktadır.

Kaba Testler

Uygulamayı F5 ile çalıştırdığımızda ve örnek bazı veriler girdiğimizde kayıt işleminin başarılı bir şekilde gerçekleştirildiğine dair bir mesaj kutusu ile karşılaşırız. Aynen aşağıdaki ekran görüntüsündeki gibi.

[![wrp_6](/assets/images/2013/wrp_6_thumb.png)](/assets/images/2013/wrp_6.png)

Ki eklenme işleminin başarılı olup olmadığını hemen veritabanına bakarak öğrenebiliriz.

[![wrp_8](/assets/images/2013/wrp_8_thumb.png)](/assets/images/2013/wrp_8.png)

Eğer ters bir durum oluşursa (örneğin form boş iken Save etmeye çalışmak gibi), bu durumda bir çalışma zamanı hatası alınacak ama durum Selim Usta’ ya yumuşatılarak iletilecektir

![Laughing out loud](/assets/images/2013/wlEmoticon-laughingoutloud_8.png)

[![wrp_7](/assets/images/2013/wrp_7_thumb.png)](/assets/images/2013/wrp_7.png)

Build Sonrası

Aslında uygulamanın Build sonrası durumuna baktığımızda aşağıdaki şekilde görülen içeriğin üretildiğini fark edebiliriz.

[![wrp_9](/assets/images/2013/wrp_9_thumb.png)](/assets/images/2013/wrp_9.png)

Word Document projesi, ProductDocument.docx haricinde, Solution da kullanılan başka dosyaları da doğal olarak içerecektir. Tabi burada akla gelen ilk soru bu içeriği nasıl dağıtılabileceğidir? Development yapılan makinede herhangibir sorun olmayacaktır. Debug veya Release klasöründeki ProductDocument.dox dosyasının çalıştırılması yeterlidir. Ancak bu çözümü Selim Usta’ nın bilgisayarına yüklemek için en azından bir Setup paketine sahip olmak dağıtım işini kolaylaştıracaktır.

Bunun için Publish işlemi Word Document projesi için uygulanabilir. Burada Click Once teknolojisinden yararlanılabildiğini de belirtelim. Bir Publish paketini söz konusu uygulama için nasıl hazırlayabileceğinizi [MSDN üzerindeki bu adresten öğrenebilirsiniz](http://msdn.microsoft.com/en-us/library/vstudio/bb772100.aspx). Detayları ayrı bir makale konusu olacağından burada derinlemesine incelemedik.

> İlgili adresteki içeriği dikkatlice okumanızı ve özellikle Post-Deployment kullanımına bakmanızı öneririm. Nitekim Post-Deployment sırasında, ilgili Word dosyasının bir kopyasının istemci bilgisayarda istenen bir klasöre atanması işlemi gerçekleştirilebilmektedir. Ayrıca Publisher’ ın Trusted olarak kabul edilebilmesi için gerekli Signing işlemlerini de atlamayın derim
>
> ![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_184.png)

Publish işlemi sonrası aşağıdaki ekran görüntüsündekine benzer bir içerik oluşacaktır. Setup dosyasını kullanarak tipik bir install işlemi gerçekleştirebilirsiniz. Ayrıca bu klasörde yer alan ProductDocument çalıştırıldığında doğrudan tasarlamış olduğumuz Form’ un açıldığını görebiliriz.

[![wrp_10](/assets/images/2013/wrp_10_thumb.png)](/assets/images/2013/wrp_10.png)

Sonuç

Geliştirdiğimiz örnek aslında Document Level tipinden kabul edilen bir proje uygulamasıdır. İstenirse Application Level şeklinde bir proje de üretilebilir ki bu durumda tüm Word dokümanları için uygulama seviyesinde bir geliştirme yapma şansına sahip olabiliriz. Elbette yazımıza konu olan ve Selim Usta’ nın işine yarayacağını düşündüğümüz çözümün geliştirilmesi gereken pek çok yeri vardır. Örneğin,

- Form üzerinde bir doğrulama (Validation) mekanizması mevcut değildir. Çok doğal olarak Selim Usta sayısal olması gereken alanlara metinsel içerik girebilir.
- Word dosyası üzerinden yeni bir ürün eklendiğinde belki form temizlenebilir ve hatta belki de alt sayfalarda yer alan bir DataGridView kontrolü içerisine, yapılan ekler basılarak özet bir durum raporu sunulabilir.
- Selim Usta dalgınlıkla yanlış bir içerik girebilir. Bu durumda ilgili kayıtları silmek isteyecektir. Bu vakanın da doküman yoluyla karşılanması gerekebilir. Ya da mevzunun Rukiye’ ye iletilmesi yolu tercih edilebilir ![Smile](/assets/images/2013/wlEmoticon-smile_87.png)
- Bir WCF servis katmanının olması da çok daha iyi olabilir. Nitekim bu sayede Word Document projesini kullanan herhangibir istemci bilgisayarının sadece servise ulaşması yeterli olacaktır. Ki servis de Host edildiği makinede Entity Framework’ ü kullanarak bir Repository’ ye yazma işlemini icra edebilir ve bu sayede Word dosyasının olduğu bilgisayar için Loosely Coupled bir geçerlilik de sağlanmış olur.

Bu ve benzeri eksiklikleri siz değerli okurlarıma bırakıyorum. Diğer yandan senaryoyu biraz daha farklılaştırabilirsiniz. Söz gelimi TFS Client Object Model’ i de işin içerisine katabilir ve bir Word Template’ i üzerinden Product Backlog Item ve Task girişlerini yaptırabilirsiniz. Bu ödev için aşağıdaki grafiği göz önüne alabilirsiniz.

[![wrp_11](/assets/images/2013/wrp_11_thumb.png)](/assets/images/2013/wrp_11.png)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Winking smile](/assets/images/2013/wlEmoticon-winkingsmile_184.png)

[HowTo_Word.zip (1,80 mb)](/assets/files/2013/HowTo_Word.zip)
[Dosya boyutunun küçülmesi için Packages ve Release klasörleri silinmiştir]

[Orjinal Yazım Tarihi 03-15-2013]