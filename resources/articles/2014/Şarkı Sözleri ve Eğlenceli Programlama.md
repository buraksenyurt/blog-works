---
title: "Şarkı Sözleri ve Eğlenceli Programlama"
pubDate: 2014-01-16 16:36:00
categories:
  - WPF
tags:
  - WPF
  - XML
  - JavaScript
  - C#
  - .NET
  - ASP.NET
  - LINQ
  - Windows Forms
  - XAML
  - SOAP
  - REST
  - JSON
  - Web API
  - Web Service
  - HTTP
  - Async/Await
  - Generics
---

# Şarkı Sözleri ve Eğlenceli Programlama
[![scorpions-the-millenium-collection](images/scorpions-the-millenium-collection_thumb.jpg)](images/scorpions-the-millenium-collection.jpg) Merhaba Arkadaşlar,

Geçtiğimiz gün standart olarak Youtube üzerinden gerek 80ler, gerek 90lara ait iz bırakan sanatçıları ve şarkılarını izlemekteydim. Çok sık yaptığım şeylerden birisi de bu şarkıları sosyal ağda paylaşmak aslında. Ama bazende şarkıların melodileri dışında sözlerini de mırıldanmaktayım kendi kendime, ki pek çoğumuzun bunu sıkça yaptığından eminim

![Smile](images/wlEmoticon-smile_43.png)

Fark ettim ki, pek çok şarkının sözünü unutuyorum/unutmuşum. Hatırlamak için de internet üzerinden Googlelamam gerekiyor. Gerçi bununla ilişkili belli başlı siteler de var ve onları da kullanabilirim ama elimde basit bir program arayüzü olsa çok daha etkili olabilir.

Mesela bir Windows Forms veya WPF (Windows Presentation Foundation) uygulaması olsa. Internete bağlanabildiği sürece istediğim sanatçının istenen albümündeki istediğim şarkının sözlerini getirse

![Winking smile](images/wlEmoticon-winkingsmile_108.png)

İşte bu amaçla çıktım yola ve basit bir uygulama geliştirmek üzere oturdum bilgisayarımın başına.

LyricWiki

[LyricWiki](http://api.wikia.com/wiki/LyricWiki_lyrics) isimli şarkı sözlerine ait detaylı bir içeriğe sahip olan site, dış dünyaya da servisler aracılığıyla destek vermekte. SOAP (Simple Object Access Protocol) bazlı servisler kullanılabileceği gibi REST (Representational State Transfer) API tarzındaki hizmetler yardımıyla da şarkı aramaları yapılabilmekte. Lyrics Wikia dan örnek servis çağrısı kullanımı, Linkin Park gurubu için aşağıdaki gibidir.

[http://lyrics.wikia.com/api.php?func=getArtist&artist=Linkin_Park](http://lyrics.wikia.com/api.php?func=getArtist&artist=Linkin_Park)

[![lyricapi_1](images/lyricapi_1_thumb.png)](images/lyricapi_1.png)

Peki bir şarkının sözlerini nasıl alabiliriz?

Örneğin Linkin Park’ ın 1997 yılı Xero albümündeki Fuse isimli şarkının sözlerini Text, HTML, XML veya JSON formatlarında almak istediğimizi düşünelim. Bu durumda URL sorgularımızın aşağıdaki gibi olması yeterlidir. Dikkat edileceği üzere fmt parametresinin değiştirilmesi, istenen formatta (HTML, Text, JSON, XML) bir çıktı alınması için yeterlidir.

HTML (Hyper Text Markup Language)

[http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=html](http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=html) için

[![lyricapi_2](images/lyricapi_2_thumb.png)](images/lyricapi_2.png)

Text

[http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=text](http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=text) için

"Of course you know what a fuse is... It's a long piece of cord impregnated with gun powder. When you strike a match and light it It burns, fitfully, spiraling to its end At which there is, a little surprise..." From the planet of Krypton Short suit MCs you will be ripped on (ripped on) You fell off and it's my lyric sheet you slipped on Get[...]

XML (eXtensible Markup Language)

[http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=xml](http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=xml) için

```xml
<LyricsResult> 
<artist>Linkin Park</artist> 
<song>Fuse</song> 
<lyrics> 
"Of course you know what a fuse is... It's a long piece of cord impregnated with gun powder. When you strike a match and light it It burns, fitfully, spiraling to its end At which there is, a little surprise..." From the planet of Krypton Short suit MCs you will be ripped on (ripped on) You fell off and it's my lyric sheet you slipped on Get[...] 
</lyrics> 
<url>http://lyrics.wikia.com/Linkin_Park:Fuse</url> 
<page_namespace>0</page_namespace> 
<page_id>459941</page_id> 
<isOnTakedownList>0</isOnTakedownList> 
</LyricsResult>
```

JSON (JavaScript Object Notation)

[http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=js](http://lyrics.wikia.com/api.php?func=getSong&artist=Linkin_Park&song=Fuse&fmt=js) için

```javascript
function lyricwikiSong(){ 
this.artist='Linkin Park'; 
this.song='Fuse'; 
this.lyrics='"Of course you know what a fuse is...\nIt\'s a long piece of cord impregnated with gun powder.\nWhen you strike a match and light it\nIt burns, fitfully, spiraling to its end\nAt which there is, a little surprise..."\n\nFrom the planet of Krypton\nShort suit MCs you will be ripped on (ripped on)\nYou fell off and it\'s my lyric sheet you slipped on\nGet[...]'; 
this.url='http://lyrics.wikia.com/Linkin_Park:Fuse'; 
} 
var song = new lyricwikiSong();
```

şeklinde sonuçlar elde ederiz.

Peki bu tip bir kullanım söz konusu ise.Net tarafında ilgili içerikleri kullanarak kendimiz için eğlenceli bir program geliştirebilir miyiz acaba?

![Winking smile](images/wlEmoticon-winkingsmile_108.png)

Örnek Uygulama

Söz gelimi bir WPF uygulaması yazsak ve aradığımız bir şarkı sözünü bulmak için gerekli işlevsellikleri burada sağlamaya çalışsak. Öncelikle aşağıdaki gibi bir arayüz tasarımı ile işe başlayabiliriz diye düşünüyorum.

[![lyricapi_3](images/lyricapi_3_thumb.png)](images/lyricapi_3.png)

WPF tabanlı uygulamamızın ana formuna ait XAML içeriği ise aşağıdaki gibi geliştirilebilir.

```xml
<Window x:Class="LyricsDotCom.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="MainWindow" Height="600" Width="640"> 
    <StackPanel> 
        <TextBlock Text="Bir Grup/Şarkıcı adı giriniz" FontWeight="Bold" Margin="2,2,2,2"/> 
        <TextBox x:Name="txtSinger" Text="Linkin Park" Margin="2,2,2,2" FontWeight="Bold"/> 
        <Button x:Name="btnFindSinger" Content="Albümlerini Getir" Margin="2,2,2,2" Width="120" HorizontalAlignment="Right" Click="btnFindSinger_Click"/> 
        <StackPanel Orientation="Horizontal" DataContext="{Binding}" x:Name="panelAlbums"> 
            <ListBox x:Name="lstAlbums" Width="310" ItemsSource="{Binding}" IsSynchronizedWithCurrentItem="True" Height="200"> 
                <ListBox.ItemTemplate> 
                    <DataTemplate> 
                        <StackPanel> 
                            <TextBlock Text="{Binding Path=Name}"/> 
                            <TextBlock Text="{Binding Path=Year}"/> 
                        </StackPanel> 
                    </DataTemplate> 
                </ListBox.ItemTemplate> 
            </ListBox> 
            <ListBox x:Name="lstAlbumSongs" SelectionChanged="lstAlbumSongs_SelectionChanged" Width="310" Height="200" ItemsSource="{Binding Path=Songs}"/>            
        </StackPanel> 
        <TextBlock x:Name="txtLyric"/> 
    </StackPanel> 
</Window>
```

Kullanıcılar text box kontrolüne bir grup veya şarkıcı adı yazarak işe başlarlar. Düğmeye basıldıktan sonra eğer söz konusu gruba ait bir albüm içeriği varsa, sol taraftaki ListBox kontrolüne otomatik olarak doldurlur. Ardından herhangibir albüm seçilir. Seçilen albüm içerisinde yer alan şarkıların listesi ise sağ tarafta yer alan ListBox kontrolü içerisinde gösterilir. Kullanıcı buradan herhangi bir şarkı seçtiğinde ise, bu şarkıya ait sözler alt tarafta yer alan TextBox bileşeni içerisine basılır.

XAML içeriğinde görüldüğü üzere ListBox kontrolleri aslında birbirleri ile veri bağlanması açısından ilişkilidirler. Bu sebepten dolayı onları içeren StackPanel kontrolünün DataContext özelliği kullanılmıştır. lstAlbums isimli kontrol, doğrudan panele bağlanan veri içeriğini gösterecek şekilde bir DataTemplate kullanmaktadır. Buna göre albümlerin adları ve yayınlandıkları yıl bilgileri lstAlbums kontrolünde öğeler içerisinde gösterilmektedir. lstAlbumSongs ise yine StackPanel bileşeninin veri içeriğini kullanmaktadır ancak Path özelliğine dikkat edilecek olursa, bağlanan öğelerin Songs isimli koleksiyonlarını göstermektedir.

XAML içeriğindeki veri bağlama işlemleri ile REST sorgularının gerçekleştirildiği arka plan kodları ise (ki button arkası diyebilirim doğrudan) aşağıda görüldüğü gibidir.

```csharp
using System; 
using System.Collections.Generic; 
using System.Configuration; 
using System.IO; 
using System.Net; 
using System.Windows; 
using System.Windows.Controls; 
using System.Windows.Documents; 
using System.Xml; 
using System.Xml.Linq;

namespace LyricsDotCom 
{ 
    public partial class MainWindow 
        : Window 
    { 
        #region Genel değişkenler

        string singerQuery = @"http://lyrics.wikia.com/api.php?func=getArtist&artist={0}&fmt=xml"; 
        string songQuery = @"http://lyrics.wikia.com/api.php?func=getSong&artist={0}&song={1}&fmt=text"; 
        string proxyName = ConfigurationManager.AppSettings["proxyName"]; 
        string proxyPort = ConfigurationManager.AppSettings["proxyPort"]; 
        string proxyUsername = ConfigurationManager.AppSettings["proxyUsername"]; 
        string proxyPassword = ConfigurationManager.AppSettings["proxyPassword"]; 
        List<Album> albums = null; 
        WebClient webClient = null;

        #endregion

        public MainWindow() 
        { 
            InitializeComponent(); 
            webClient = GetWebClient(); 
        }

        private void btnFindSinger_Click(object sender, RoutedEventArgs e) 
        { 
            MemoryStream memoryStream = GetMemoryStream(string.Format(singerQuery, txtSinger.Text)); 
            XmlTextReader reader = new XmlTextReader(memoryStream); 
            XDocument document = XDocument.Load(reader); 
            reader.Close(); 
            memoryStream.Close(); 
            if (document.Root.HasElements) 
            { 
                albums = new List<Album>(); 
                foreach (var album in document.Root.Elements("albums").Elements("album")) 
                { 
                    Album albm = new Album(); 
                    albm.Artist = txtSinger.Text; 
                    albm.Name = album.Value.ToString(); 
                    albm.Year = ((XElement)album.NextNode.NextNode).Value; 
                    albm.AmazonLink = ((XElement)album.NextNode.NextNode.NextNode.NextNode).Value; 
                    List<string> songList = new List<string>(); 
                    XElement songs = (XElement)album.NextNode.NextNode.NextNode.NextNode.NextNode.NextNode; 
                    foreach (var item in songs.Elements("item")) 
                    { 
                        songList.Add(item.Value); 
                    } 
                    albm.Songs = songList; 
                    albums.Add(albm); 
                }

                panelAlbums.DataContext = albums; 
            } 
        } 
        
        private void lstAlbumSongs_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { 
            Album selectedAlbum = lstAlbums.SelectedItem as Album;

            if (lstAlbumSongs.SelectedItem != null) 
            { 
                MemoryStream memoryStream = GetMemoryStream(string.Format(songQuery, selectedAlbum.Artist, lstAlbumSongs.SelectedItem.ToString())); 
                StreamReader reader = new StreamReader(memoryStream); 
                txtLyric.Text = reader.ReadToEnd(); 
                reader.Close(); 
                memoryStream.Close(); 
            } 
            else 
                txtLyric.Text = string.Empty; 
        }

        private MemoryStream GetMemoryStream(string query) 
        { 
            MemoryStream memoryStream = new MemoryStream(webClient.DownloadData(query)); 
            return memoryStream; 
        }

        private WebClient GetWebClient() 
        { 
            WebProxy proxy = new WebProxy(proxyName, Convert.ToInt32(proxyPort)); 
            proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword); 
            WebClient client = new WebClient(); 
            client.Proxy = proxy; 
            return client; 
        } 
    } 
}
```

Wikia’ nın Lyrcis servisine ait REST arayüzüne atılacak sorgular için kod tarafında WebClient tipinden yararlanıldığı görülmektedir. Söz konusu sistemde bir de WebProxy kullanımı mevcuttur. Nitekim uygulamanın yazıldığı sistemde bir Proxy ile internete çıkış gerçekleştirilmektedir. WebProxy için gerekli olan Proxy adı, port numarası, kullanıcı ve şifre bilgileri ise App.config dosyasındaki appSettings bölümünden ConfigurationManager tipi yardımıyla okunmaktadır.

> ConfigurationManager sınıfının kullanılabilmesi için projeye System.Configuration assembly’ ını referans etmeyi unutmayın.

WebClient sınıfına ait nesne örneği kullanılarak albüm listesinin alınması ve bir şarkının sözlerinin getirilmesi için iki farklı REST sorgusu gönderilmektedir. Sorgu sonuçları bir MemoryStream içerisine alınmakta ve duruma bağlı olarak XmlTextReader ya da StreamReader yardımıyla okunmaktadır.

Uygulamanın en çok zorlayan kısımlarından birisi de, albüm listelerinin getirildiği XML içeriğinin nesnel olarak ayırştırıldığı kısmıdır (Parsing). Nedense albums elementi içerisinde alt element olarak album elementlerinin olması beklenirken, albums elementi ile aynı seviyede kullanılan album elementlerinin olduğu bir XML şeması söz konusudur

![Confused smile](images/wlEmoticon-confusedsmile_21.png)

Neden bu şekilde bir servis üretimi gerçekleştirildiğini pek bilemiyorum (en azından yazının hazırlandığı tarih itibariyle) açıkçası ama bana kalsaydı sanırım şemayı bu şekilde tasarlamazdım.

Uygulamamızda görüldüğü üzere Album isimli bir POCO (Plain Old Clr Objects) tipi kullanılmaktadır.

[![lyricapi_4](images/lyricapi_4_thumb.png)](images/lyricapi_4.png)

```csharp
using System.Collections.Generic;

namespace LyricsDotCom 
{ 
    public class Album 
    { 
        public string Name { get; set; } 
        public string Year { get; set; } 
        public string AmazonLink { get; set; } 
        public List<string> Songs { get; set; } 
        public string Artist { get; set; } 
    } 
}
```

Aslında şarkılara XML, JSON gibi içerikler ile ulaşılmak istenirse bir Song tipinin de tasarlanması düşünülebilir. Özellikle şarkı sözlerinin tamamının bulunduğu web sayfası linki bu şekilde elde edilebilir.

> Lisanslama kuralları gereği bazı şarkı sözlerinin sadece 7de1 inin çekilebildiği belirtilmiştir ([Bu adresteki](http://api.wikia.com/wiki/LyricWiki_API/SOAP) Cropped Lyrics başlığını okuyunuz) Dolayısıyla uygulamamızda pek çok şarkı sözü eksik olarak görünmemektedir ama hatırlatıcı olması açısından bu da bir şeydir (Daha iyi bir şarkı söz REST servisini aramaktayım. Siz de arayın ![Smile](images/wlEmoticon-smile_43.png))

Album tipi içerisinde oldukça yararlı bilgiler bulunmaktadır. Söz gelimi albümün çıkış tarihi ve Amazon sitesinden doğrudan arama sorgusu gibi. Dolayısıyla istenirse hemen Amazon sepetinize ekleyebilirsiniz de. Amazon Web Servisler ile konuşan bir ara katman bile olabilir.

[![lyricapi_5](images/lyricapi_5_thumb.png)](images/lyricapi_5.png)

Örneğin Debug modda yakadlığımız bir albüm için gelen amazon arama sorgusu aşağıdaki gibidir.

[http://www.amazon.com/exec/obidos/redirect?link_code=ur2&tag=wikia-20&camp=1789&creative=9325&path=external-search%3Fsearch-type=ss%26index=music%26keyword=Linkin%20Park%20Underground%204.0](http://www.amazon.com/exec/obidos/redirect?link_code=ur2&tag=wikia-20&camp=1789&creative=9325&path=external-search%3Fsearch-type=ss%26index=music%26keyword=Linkin%20Park%20Underground%204.0)

Şimdi uygulamamızı test sürüşüne çıkartabiliriz. Bu amaçla Scorpions grubuna ait bir parçanın sözlerini çekmeye çalışalım. “Still loving you” mesela

![Winking smile](images/wlEmoticon-winkingsmile_108.png)

[![lyricapi_6](images/lyricapi_6_thumb.png)](images/lyricapi_6.png)

Tabi restriction nedeni ile sadece bir bölümünü görebildik ama sonuçta geliştirdiğimiz örnekte amacımız Developer profili açısından bakıldığında REST tabanlı bir servisi basit yöntemler ile nasıl kullanabileceğimiz ve bir WPF uygulaması içerisinde ilgili kontrollere nasıl bağlayabileceğimiz idi. Dolayısıyla istediğimiz ürün faydasını tam olarak sağlayamamış olsakta geliştirme (development) adına bir kaç fikir sahibi olduğumuzu düşünebiliriz.

Peki bundan sonrası için neler yapılabilir?

- Öncelikli olarak Developer API Key ile şarkı sözlerinin tamamının çekilebilip çekilemediğine bakılmalıdır ancak pek çok şarkının lisansı henüz alınamadığı için sadece bir kısmı görünecektir.
- Şarkılar için Song isimli bir POCO tip geliştirilip içerisine tüm içeriği gösteren web sayfasına yönlendirme yapacak bağlantının basılması sağlanabilir.
- Uygulama bir ASP.NET Web User Control olarak da sunulabilir.
- Bu kısımları ciddi anlamda düşünmenizi ve yapmaya çalışmanızı öneririm.
- Var olan uygulamadaki çağrılar async ve await anahtar kelimeleri ile birlikte değerlendirilip asenkron hale de getirilebilir.
  - Uygulama içerisinde bir WebBrowser kontrolü de kullanılarak şarkı içeriğinin tarayıcıda açılması da sağlanabilir. Aşağıdaki gibi ![Winking smile](images/wlEmoticon-winkingsmile_108.png)[![lyric_last](images/lyric_last_thumb.png)](images/lyric_last.png)

Hoşunuza gitti mi? Öyleyse…

Yazımızın bu kısmına kadar yapmış olduğumuz örnekte görüldüğü gibi internet üzerinden Web API’ leri kullanarak dış dünyaya sunulan ücretsiz (ve bazen de kısmen ücretsiz) bilgileri alabilir ve kullanışlı hale getirebiliriz. Tabi bu tip hizmetleri sunan başka alanlarda bulunmaktadır. Örneğin bunlardan birisi IMDB (InternationalMovieDataBase) dir

![Winking smile](images/wlEmoticon-winkingsmile_108.png)

Aşağıdaki örnek kod parçasını yukarıdaki konu anlatımı üzerine kaymak niyetinde sürebilirsiniz. Aynı teknikleri kullanıyoruz ancak farklı bir içeriği ele alıyoruz.

Önce WPF Windows penceresine ait XAML içeriği

```xml
<Window x:Class="IMBDGadget.MainWindow" 
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        Title="MainWindow" Height="350" Width="480"> 
    <StackPanel> 
        <TextBlock Text="Movie Name" Margin="5,5,5,5" FontWeight="Bold"/> 
        <TextBox x:Name="txtMovieName" Margin="5,5,5,5"/> 
        <Button x:Name="btnFind" Content="Find" Width="120" Height="40" Margin="5,5,5,5" HorizontalAlignment="Right" Click="btnFind_Click"/> 
        <Grid DataContext="{Binding}" x:Name="grdMovie"> 
            <Grid.ColumnDefinitions> 
                <ColumnDefinition Width="100"/> 
                <ColumnDefinition/> 
            </Grid.ColumnDefinitions> 
            <Grid.RowDefinitions> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
                <RowDefinition/> 
            </Grid.RowDefinitions> 
            <TextBlock Text="IMDB ID" Grid.Row="0" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtImdbId" Margin="1,1,1,1" Text="{Binding Path=ImdbId}" Grid.Row="0" Grid.Column="1"/> 
            <TextBlock Text="Url" Grid.Row="1" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtImdbUrl" Margin="1,1,1,1" Text="{Binding Path=Url}" Grid.Row="1" Grid.Column="1"/> 
            <TextBlock Text="Genres" Grid.Row="2" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtGenres" Margin="1,1,1,1" Text="{Binding Path=Genres}" Grid.Row="2" Grid.Column="1"/> 
            <TextBlock Text="Languages" Grid.Row="3" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtLanguages" Margin="1,1,1,1" Text="{Binding Path=Languages}" Grid.Row="3" Grid.Column="1"/> 
            <TextBlock Text="Country" Grid.Row="4" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtCountry" Margin="1,1,1,1" Text="{Binding Path=Country}" Grid.Row="4" Grid.Column="1"/> 
            <TextBlock Text="Votes" Grid.Row="5" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtVotes" Margin="1,1,1,1" Text="{Binding Path=Votes}" Grid.Row="5" Grid.Column="1"/> 
            <TextBlock Text="Rating" Grid.Row="6" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtRating" Margin="1,1,1,1" Text="{Binding Path=Rating}" Grid.Row="6" Grid.Column="1"/> 
            <TextBlock Text="Runtime" Grid.Row="7" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtRuntime" Margin="1,1,1,1" Text="{Binding Path=Runtime}" Grid.Row="7" Grid.Column="1"/> 
            <TextBlock Text="Title" Grid.Row="8" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtTitle" Margin="1,1,1,1" Text="{Binding Path=Title}" Grid.Row="8" Grid.Column="1"/> 
            <TextBlock Text="Year" Grid.Row="9" Grid.Column="0" FontWeight="Bold"/> 
            <TextBlock x:Name="txtYear" Margin="1,1,1,1" Text="{Binding Path=Year}" Grid.Row="9" Grid.Column="1"/> 
        </Grid>            
        </StackPanel> 
</Window>
```

Ardından biraz kodlama,

Movie POCO sınıfı;

```csharp
namespace IMBDGadget 
{ 
    class Movie 
    { 
        public string ImdbId { get; set; } 
        public string Url { get; set; } 
        public string Genres { get; set; } 
        public string Languages { get; set; } 
        public string Country { get; set; } 
        public string Votes { get; set; } 
        public string Rating { get; set; } 
        public string Runtime { get; set; } 
        public string Title { get; set; } 
        public string Year { get; set; } 
    } 
}
```

ve birazcık daha kod

![Smile](images/wlEmoticon-smile_94.png)

Windows sınıfı

```csharp
using System; 
using System.Configuration; 
using System.IO; 
using System.Net; 
using System.Windows; 
using System.Xml; 
using System.Xml.Linq;

namespace IMBDGadget 
{ 
    public partial class MainWindow : Window 
    { 
        #region Genel değişkenler

       string imdbQuery = @"http://www.deanclatworthy.com/imdb/?q={0}&type=xml"; 
        string proxyName = ConfigurationManager.AppSettings["proxyName"]; 
        string proxyPort = ConfigurationManager.AppSettings["proxyPort"]; 
        string proxyUsername = ConfigurationManager.AppSettings["proxyUsername"]; 
        string proxyPassword = ConfigurationManager.AppSettings["proxyPassword"]; 
        WebClient webClient = null;

        #endregion

        public MainWindow() 
        { 
            InitializeComponent(); 
            webClient = GetWebClient(); 
        }

        private MemoryStream GetMemoryStream(string query) 
        { 
            MemoryStream memoryStream = new MemoryStream(webClient.DownloadData(query)); 
            return memoryStream; 
        }

        private WebClient GetWebClient() 
        { 
            WebProxy proxy = new WebProxy(proxyName, Convert.ToInt32(proxyPort)); 
            proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword); 
            WebClient client = new WebClient(); 
            client.Proxy = proxy; 
            return client; 
        }

        private void btnFind_Click(object sender, RoutedEventArgs e) 
        { 
            MemoryStream memoryStream = GetMemoryStream(string.Format(imdbQuery, txtMovieName.Text)); 
            XmlTextReader reader = new XmlTextReader(memoryStream); 
            XDocument document = XDocument.Load(reader); 
            reader.Close(); 
            memoryStream.Close(); 
            if (document.Root.HasElements) 
            { 
                Movie movie = new Movie(); 
                movie.ImdbId = document.Root.Element("imdbid").Value; 
                movie.Url = document.Root.Element("imdburl").Value; 
                movie.Genres = document.Root.Element("genres").Value; 
                movie.Languages = document.Root.Element("languages").Value; 
                movie.Country = document.Root.Element("country").Value; 
                movie.Votes = document.Root.Element("votes").Value; 
                movie.Rating = document.Root.Element("rating").Value; 
                movie.Runtime = document.Root.Element("runtime").Value; 
                movie.Title = document.Root.Element("title").Value; 
                movie.Year = document.Root.Element("year").Value;

                grdMovie.DataContext = movie; 
            } 
        } 
    } 
}
```

ve işte sonuç

[![lyric_imdb](images/lyric_imdb_thumb.png)](images/lyric_imdb.png)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim

![Winking smile](images/wlEmoticon-winkingsmile_108.png)

[LyricsDotCom.rar (75,84 kb)](assets/LyricsDotCom.rar)
