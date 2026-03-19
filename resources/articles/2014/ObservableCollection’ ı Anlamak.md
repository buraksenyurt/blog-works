---
title: "ObservableCollection’ ı Anlamak"
pubDate: 2014-05-01 13:59:00
categories:
  - WPF
tags:
  - WPF
  - C#
  - .NET
  - NoSQL
  - WCF
  - Workflow Foundation
  - XAML
  - HTTP
  - Generics
  - Visual Studio
---

# ObservableCollection’ ı Anlamak
[![Formspring.me](images/Formspring.me_thumb.jpg)](images/Formspring.me.jpg) Merhaba Arkadaşlar,

31 Mart 2013 deki kapanma kararına kadar Formspring ‘in sadık kullanıcılarından birisiydim. Her ne kadar anlık bir soru-cevap ortamı olmasa da, takip edenler açısından faydalı bir sosyal ağ idi. Özellikle Facebook, Twitter gibi eklentileri de, cevapların farklı sosyal ağlara bağlanmasında önemli rol oynuyordu. Bu sayede verilen cevapların daha fazla kitleye ulaşması mümkündü. Ama maya bir şekilde tutmadı, kullanıcı sayısı git gide azaldı ve sonunda kapatılma kararı verildi.(Şu anda o adrese girmek isterseniz aslında [şu adrese yönleniyor](http://new.spring.me/) ve yeni bir oluşumla karşılaşıyorsunuz)

İşte o dönemlerde WCF tarafında Interceptor'ların nasıl kullanıldığına dair bir makale talebi almıştım Formspring üzerinden. [Onu geçtiğimiz zamanlarda cevaplamayı başardım](../2012/WCF Interceptors.md). Derken bunun ardından benzer bir soru daha gelmişti. Someone'dan gelen soru şöyleydi ve henüz cevaplamayı başaramamıştım…

> burak abi merhaba senden wpf de sık kullanılan observablecollection konusunu anlatmanı rica ediyorum mmalesef bu konuda derinlemesine anlatım yapan türkçe kaynak yok saygılarımla başarılar hayırlı işler

İşte bu makalemizde söz konusu soruya cevap bulmaya çalışıyor olacağız.

WPF (Windows Presentation Foundation) bilindiği üzere Microsoft.Net Framework 3.0 ile birlikte tanıtılmış bir alt yapı (Infrastructure). Windows tabanlı masaüstü uygulamalarına (ve hatta Browser tabanlı da çalışabiliyorlar) yeni bir soluk getiren yapının XAML (eXtensible Application Markup Language) ile olan sıkı bir ilişkisi de bulunmakta. Dolayısıyla anlatacağımız konu aslında çok uzun zamandır var olan bir mevzu, lakin WPF tarafına yeni başlayan birisi için de epey yabancı sayılabilir. İşe ilk olarak bu koleksiyona olan ihtiyacı ortaya koyarak başlamakta yarar var.

Gereksinim

Günümüz yazılım ürünlerinin pek çoğu ister web tabanlı olsunlar, ister mobil cihaz üzerinde koşsunlar vb, genellikle Data-Centric (Veri odaklı) olarak geliştirilmekteler. İçerik bir veritabanı sunucusundan (hatta NoSQL tabanlı bir kaynak bile olabilir) gelebileceği gibi, bellek üzerinde oluşturulmuş bir koleksiyon veya basit bir POCO (Plain Old CLR Object) tipi dahi olabilir.

Bu açıdan bakıldığında veri odaklı uygulamaların ön yüzlerinin (User Interface) veri ile olan iletişiminde karşılıklı olarak bir bilgi transferi söz konusudur. Yani arayüzler, veride meydana gelecek en ufak bir değişiklikten haberdar olmak isterlerken, arayüzde meydana gelen değişikliklerin de veri tarafına yansıtılması gibi bir ihtiyaç ortaya çıkmış durumdadır.

> Şimdi burada durup biraz daha derin düşünmemiz gerekiyor. Tarafların birisinde meydana gelen değişiklikler sonucu başka bir tarafın/tarafların uyarılması (Notify edilmesi diyelim) yazılım dünyasında çok sık rastlanan bir durum olsa gerek. İşte bu sebepten zaten bir tasarım kalıbı bile ortaya çıkmış. Observer Design Pattern. Bu konuda daha önceden [yazdığım bir makaleye şu adresten ulaşabilirsiniz](https://www.buraksenyurt.com/post/Tasarc4b1m-Desenleri-Observer.aspx)

Observer Tasarım Kalıbı ile Olan İlişki

Peki bu desenin konumuzla ilgisi nedir? Sadece kök kelime isim benzerliği olabilir mi? Aslında pek değil. ObservableCollection'un iç yapısına bakıldığında Observer Tasarım Kalıbını uyguladığını fark edebiliriz. Çünkü bu koleksiyonun en büyük özelliği, veri bağlı kontroller ile ilişkilendirildiğinde ekleme, çıkartma ve tazeleme gibi işlemlerde uyarı verilmesine zemin hazırlıyor olmasıdır. Bu uyarı genellikle bir arayüz kontrolünün durum değişikliğinden haberdar olması olarak algılanır. Örneğin koleksiyona bir veri eklendiğinde, bu koleksiyon ile ilişkili kontrolün ilgili öğeyi otomatik olarak göstermesi gibi.

ObservableCollection sınıfı ve çevresinde etkileşimde olduğu tiplerin genel bir fotoğrafı şekilde görüldüğü gibidir.

[![htoc_1](images/htoc_1_thumb.png)](images/htoc_1.png)

System.Collections.ObjectModel isim alanı (namespace) içerisinde yer alan generic ObservableCollection sınıf, iki arayüzü (Interface) implemente etmektedir. Bunlardan birisi özellik bazlı (Property Based) değişiklikler sonrası tetiklenecek olayı (Event) uygulatan INotifyPropertyChanged iken, diğeri de koleksiyon değişimleri sonrası tetiklenmesi gereken olayı uygulatan INotifyCollectionChanged arayüzüdür (Interface) Zaten bu iki arayüzün belirttiği olaylar ObservableCollection içerisinde uygulanırken, bağlandıkları kontrolleri uyaracak şekilde tasarlanmışlardır.

Öyleyse ObservableCollection ın temel amacı bellidir; Generic T tipi için söz konusu olan ekleme (Add), silme (Remove) veya yeniden tazeleme (Refresh) gibi işlemlerde bir uyarı (Notify) yayınlamak.

Örnek

Konuyu daha net anlamak adına basit bir örnek üzerinden ilerlemeye çalışalım. Öncelikli olarak aşağıdaki sınıf çizelgesinde (Class Diagram) görünen tipleri içeren bir WPF uygulaması oluşturduğumuzu düşünelim.

[![htoc_2](images/htoc_2_thumb.png)](images/htoc_2.png)

```csharp
namespace HowTo_ObservableCollection 
{ 
    public class Book 
    { 
        public int BookId { get; set; } 
        public string Title { get; set; } 
        public int PageSize { get; set; } 
        public string Producer { get; set; } 
    } 
}

using System.Collections.ObjectModel;

namespace HowTo_ObservableCollection 
{ 
    public class BookList 
        :ObservableCollection<Book> 
    { 
        public BookList() 
           :base() 
        { 
            Add( 
                new Book 
                { 
                    BookId = 1 
                    , Title = "Advanced WCF Programming" 
                    , Producer = "Maybe Me" 
                    , PageSize = 1280 
                } 
                ); 
            Add( 
                new Book 
                { 
                    BookId = 2, 
                    Title = "SOA: from a Developer Vision", 
                    Producer = "Maybe Me", 
                    PageSize = 740 
                } 
                ); 
       } 
    } 
}
```

BookList, ObservableCollection türevli olacak şekilde tanımlanmış olup içerisindeki yapıcı metod (Constructor) ile bir kaç Book nesne örneğinin yüklenmesini sağlamaktadır. Book tipi içerisinde çok basit bir kaç özelliğe yer verilmiştir.

BookList bir ObservableCollection örneğidir. Bu nedenle veri bağlı kontroller ile ilişkilendirildiğinde ekleme, silme gibi operasyonlar sonrasında bildirimlerde bulunabilir. Bu durumu test etmek ve hangi hallerde koleksiyonun nasıl çalıştığını görmek adına geliştirmekte olduğumuz WPF uygulamasının arayüzünü aşağıdaki gibi tasarlayarak devam edelim.

[![htoc_3](images/htoc_3_thumb.png)](images/htoc_3.png)

Window seviyesinde bir Resource tanımlanmış ve BookList koleksiyonu işaret edilmiştir. BookList koleksiyonunun kendisi, ListBox kontrolüne bu static resource yardımıyla ItemsSource özelliği üzerinden bağlanmaktadır.

Book tipine ait özelliklerin, DataTemplate içerisindeki kontrollerin Text özelliklerine nasıl bağlandığına dikkat etmekte yarar vardır. Bir başka kayda değer nokta ise şudur; Visual Studio'nun WPF Designer'ı üzerinde çalışılmakta olup, uygulama çalışmadığı halde koleksiyon içerisinde yer alan kitap bilgileri, kontrollerin Text özelliklerine bağlanmış ve içerikleri gösterilmiştir.

Lakin burada ayrı bir nokta daha vardır. Eğer BookList sınıfını ObservableCollection yerine List tipinden türetirsek de, az önce belirttiğimiz davranış sergilenecektir. Yani Visual Studio tasarım zamanı yine kitap bilgilerini bağlanan kontrollerde gösterecektir. O zaman ObservableCollection nin henüz kullanım amacı tam olarak tespit edilebilmiş değildir. Eğer List tipi de yukarıdaki senaryoda aynı davranışı gösterdiyse, neden ObservableCollection kullanalım ki.

Neden ObservableCollection Kullanırız ki?

Gelin örneğimizi biraz daha değiştirelim ve aşağıdaki hale getirelim.

[![htoc_4](images/htoc_4_thumb.png)](images/htoc_4.png)

```csharp
using System.Windows;

namespace HowTo_ObservableCollection 
{ 
    public partial class MainWindow 
        : Window 
    { 
        BookList sourceList = null;

        public MainWindow() 
        { 
            InitializeComponent();

            sourceList = ListBoxBooks.ItemsSource as BookList; 
        }

        private void ButtonAdd_Click_1(object sender, RoutedEventArgs e) 
        { 
           sourceList.Add(new Book 
            { 
                BookId=94, 
                 Title="Starwars Clone Wars", 
                  Producer="Lucas Arts", 
                   PageSize=180 
            } 
            ); 
        }

        private void ButtonRemove_Click_1(object sender, RoutedEventArgs e) 
        { 
           sourceList.RemoveAt(0); 
        }

        private void ButtonChange_Click_1(object sender, RoutedEventArgs e) 
        { 
            sourceList[1].Title = "Changed..."; 
        } 
    } 
}
```

İlk olarak ListBox kontrolünün çalıştığı veri kaynağını ItemsSource özelliğini BookList tipine dönüştürerek (cast) elde etmekteyiz. Nitekim ekleme, çıkartma ve özellik değiştirme işlemlerini bu kaynak üzerinden gerçekleştireceğiz.

3 farklı Button kontrolümüz ile Add, Remove ve Property Change senaryolarını ele almaya çalışıyoruz (Burada özellikle kodun ilgili yerlerine Breakpoint'ler koyup Debug ederek ilerlememiz de yarar var)

İlk olarak Add işlemine bakalım. BookList'e yeni bir Book nesne örneği eklendiğinde veriye bağlanmış olan kontrollerde otomatik olarak bu değişim için haberdar edilecek ve aşağıdaki çalışma zamanı durumu söz konusu olacaktır.

[![htoc_5](images/htoc_5_thumb.png)](images/htoc_5.png)

Görüldüğü gibi 94 numaralı Book nesne örneği koleksiyona eklendikten sonra yeni içerik ListBox kontrolüne de anında yansımıştır. Eğer Remove işlemini gerçekleştirirsek de benzer bir durum ortaya çıkacak ve ListBox kontrolü güncellendiği gibi, kaynak koleksiyondan da söz konusu kitap çıkartılacaktır.

[![htoc_6](images/htoc_6_thumb.png)](images/htoc_6.png)

Add sonrası Remove işlemi icra edildiğinde ise koleksiyondaki eleman sayısının 1 eksildiği görülecektir. Ayrıca ListBox kontrolünden de ilgili Book örneği kaldırılacaktır.

[![htoc_7](images/htoc_7_thumb.png)](images/htoc_7.png)

Peki Change işlemine gelirsek. Aslında Change vakasında, koleksiyondaki bir Book nesne örneğinin Title özelliğinde yapılan değişiklik söz konusudur. Bu durumda aşağıdaki sonuçlar ile karşılaşılır.

[![htoc_8](images/htoc_8_thumb.png)](images/htoc_8.png)

Dikkat edileceği üzere koleksiyondaki Book örneğinin Title özelliğinin içeriği Changed olarak değişmiştir. Ne varki kullanıcı arayüzüne baktığımızda aynı etkinin oluşmadığı görülür. Title ilk ve orjinal hali ile kalmıştır.

[![htoc_9](images/htoc_9_thumb.png)](images/htoc_9.png)

Bu davranış doğaldır. Nitekim INotifyPropertyChanged aslında ObservableCollection tipi için (ki örneğimizde bundan türeyen BookList) için geçerlidir. Ancak yaptığımız değişiklik, aslında bir Book nesne örneğinin özelliği üzerinde meydana gelmektedir. Bir başka deyişle Book sınıfına INotifyPropertyChanged arayüzü implemente edilmediği takdirde, User Interface'in de durum değişikliğinden haberdar olması pek mümkün değildir.

Şimdi gelelim önemli bir noktaya; Eğer BookList'i List tipinden türetirsek, Add ve Remove işlemleri sonrasında, User Interface kontrollerine ait içeriklerin güncellenmediğini görürüz. Şu anda ObservableCollection ile List arasında oluşan önemli bir davranış farkını da yakalamış bulunuyoruz.

Sanıyorum ObservableCollection tipinin kullanım amacını ve şeklini biraz daha net anlayabilmişizdir. ObservableCollection bildirim yapmak için gerekli arayüz tanımlamalarını uyguladığından, özellikle veri bağlı kontrollerin söz konusu olduğu senaryolarada, hem bileşenlerin hem de kaynak koleksiyonun Add,Remove,Refresh işlemleri sonrası uyarılmasında hazır bir alt yapı sunmaktadır. Örneği test ederken Debug ederek ilerlemenizi ve özellikle BookList koleksiyonunu ListBox türevli ele alarak analiz etmenizi öneririm.

Tabi bakabileceğiniz/araştırabileceğiniz başka vakalar da var. Örneğin,

- ListBox kontrolünün Items özelliği ile görsel içerik de değişiklikler yaparsak ne olur? Bundan asıl koleksiyon (uygulamada BookList tipinin çalışma zamanı örneği) içeriği etkilenir mi?
- Ya da bu senaryo aslında Workflow Foundation tarafında özellikle görsel içeriğe sahip olan Custom Component’ ler de göz önüne alınabilir mi? Söz gelimi, kullanılabilir WCF servislerin listesini bir kaynaktan çekip, Workflow Component’ inin içerisinde yer alan bir ListBox bileşeninde gösterebilir miyiz? vb…

Bu tip soruları da ben size sormuş olayım.

Böylece geldik bir yazımızın daha sonuna. Bir sonraki makalemizde görüşmek dileğiyle hepinize mutlu günler dilerim.

[HowTo_ObservableCollection.zip (71,13 kb)](assets/HowTo_ObservableCollection.zip)
