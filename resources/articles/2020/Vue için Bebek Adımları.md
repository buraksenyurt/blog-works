---
title: "Vue için Bebek Adımları"
pubDate: 2020-12-21 08:46:00
categories:
  - Vue.js
tags:
  - Vue.js
  - Bash
  - .NET
  - NoSQL
  - JSON
  - Web API
  - Authentication
  - Rust
  - JavaScript
  - React
  - Vue
  - Caching
  - GitHub
---

# Vue için Bebek Adımları
![image.axd](images/image.axd)

Yazılım işine girdiğimden beri en çok zorlandığım konu Frontend tarafında kodlama yapmak. Ne yazık ki sadece Backend tarafta kalma lüksümüz de pek bulunmuyor. Örneğin hali hazırda çalışmakta olduğum firmada yeni nesil birçok uygulama önyüz tarafında çeşitli Javascript çatıları (Framework) kullanıyor.

Pratikte bakınca oldukça iyi bir kurgu aslında. Önyüzü Vue, React, Angular vb yapılarla geliştirip, asıl iş kuralları için arka planda yer alan.Net Core Web API servislerine gelmek. C# ve.Net Core tarafına aşina olduğum için arka planı rahatça kodluyorum, önyüz tarafında ise önceden geliştirilmiş sayfalara bakarak bir şeyler yapabiliyorum. Yani işin özü Vue çatısının temellerinde sorunlarım var. Bu amaçla SkyNet'e uğradığım bir gün oturdum ekran başına en basit adımlarıyla bu işi nasıl öğrenirim bir kurcalayayım dedim.

Tabii öncesinde Vue.Js ile ilgili bir bilgi vermek de lazım. Açık kaynak olarak sunulan ilk Commit'i 2013 ve ilk Release'i de 2014 olan Vue, temel olarak Singe Page Application geliştirmek için Model View ViewModel (MVVM) desenini baz alan bir Javascript Framework olarak düşünülebilir. İşi gücü arayüz geliştirilmesini MVVM'in nimetlerinden yararlanarak kolaylaştırmak. Reactive olması, HTML'i direktifler ile genişletmesi ve DOM elementleri ile veriyi, olayları kolayca bağlaması öne çıkan özellikleri arasında sayılabilir.

Bu arada bugüne kadar çıkan sürümlere Manga dizilerinin adları verilmiş. Son sürüm One Piece, 2016 - Ghost in the Shell, 2015 - Dragon Ball, 2014 - Blade Runner, Cowboy Bebop ve Animatrix gibi isimlendirmeler kullanılmış (Peki Animatrix ile başlayan bu Manga adlarının alfabetik sırada gittiğini biliyor muydunuz?) İşin magazin kısmı bir yana kalsın gelin biz basit adımlarla Vue'nun temel kabiliyetlerini tanımaya çalışalım. Bunun için Javascript kütüphaneleri veya CLI araçlarını indirmemize de gerek yok. Temel konular için Vue'nun CDN (Content Delivery Network) kaynağından ([https://unpkg.com/vue](https://unpkg.com/vue)) yararlanmamız yeterli.

İlk adımımızda onun ne kadar duyarlı olduğunu (Reaktif) anlamaya çalışalım (Yorum satırlarını okumayı ihmal etmeyin)

```bash
touch vue_is_reactive.html
```

Kodlarımızı aşağıdaki gibi geliştirelim.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 01</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>
    <!--
        Vue'da DOM nesneleri ile veri(data) birbirine bağlıdır ve sürekli etkileşim halindedir.
    -->

    <div id="appComponent" style="text-align:center;">
        <h1>Şu An Çalıştığım Kitap</h1>
        <p>{{bookName}}</p>
        <p>{{startDate}}</p>
        <!-- Javascript expression içerisinde aşağıdaki gibi fonksiyon çağrıları da yapılabilir-->
        <p>{{bookName.split('').reverse().join('')}}</p>
    </div>

    <script type="text/javascript">
        /*
            app bir Vue uygulama nesnesidir. 
            Parametre olarak çeşitli seçenekleri ihtiva eden bir JSON değişkeni alır.
            el(element) niteliği uygulama nesnesini appComponent ismiyle div elementine bağlar .
            Vue yapıcı fonksiyonundaki JSON nesnesinin data özelliği içerisinde koyduğumuz alanlar, DOM içerisinde {{ }} ifadelerinin olduğu yerlerde kullanılır.
            Mustache stilindeki {{ }} yerler javascript expression olarak adlandırılır. Vue bu ifadeleri gördüğünde, data özelliğindeki karşılıkları ile değiştirir.
            Vue'nun nesne verileri(instance data) HTML'de referans edildikleri heryere bağlanır. 
            Bunu daha iyi anlamak için HTML sayfasını açtıktan sonra F12 ile Developer bölgesine geçin ve Console'da 
                app.bookName="Learning Vue"
            yazın. HTML içeriğinde bookName olan heryerin anında değiştiğini göreceksiniz. İşte bu Reactive olma özelliğidir.
        */
        var app = new Vue(
            {
                el: '#appComponent',
                data: {
                    bookName: "Rust Programming Cookbook",
                    startDate: "Today"
                }
            }
        )

    </script>
</body>

</html>
```

Oluşturduğumuz HTML sayfasını bir tarayıcıda açtıktan sonra özellikle F12 ile Debug moduna geçip Vue uygulama nesnesi olan app değişkeninin data özelliğindeki bookName içeriğini Console üstünden değiştirmeyi deneyin. Bu değişiklik sayfada bookName'i kullanan tüm elementlere yansıyacaktır. Buradan Vue ana bileşeninin (Component) DOM ile etkileşim halinde olduğunu söyleyebiliriz. İşte bu reaktif olmanın bir sonucudur.

![image.axd](images/image.axd)

ve F12 - Console sonrası.

![image.axd](images/image.axd)

İkinci adımımızda Attribute Binding konusunu ele alacağız. HTML elementlerindeki nitelikleri (Örneğin img elementinin src niteliğini) direktifler (Örnekte v-bind) ile Vue verisine (data özelliğinin değerleri) nasıl bağlayacağımızı göreceğiz.

```bash
touch vue_attribute_binding.html
```

HTML sayfa kodlarını aşağıdaki gibi yazarak devam edelim.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 02</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>
    <!--
        HTML elementlerinin niteliklerini(attribute)'de veriye bağlamak isteyebiliriz.
        Örneğin aşağıdaki HTML yapısından yer alan src ve altText niteliklerini, data nesnesinin sırasıyla coverPhoto ve alternativeText alanlarına bağlamak istediğimizi düşünelim.
        Bu durumda v-bind isimli yönergeyi(directive) kullanmamız gerekir.
        01 nolu örnekte olduğu gibi bu HTML'i açtıktan sonra tarayıcının Console penceresinde
            app.coverPhoto="./images/book_2.jpeg"
        yazın. Fotoğrafın hemen değiştiğini göreceksiniz. Yani v-bind direktifi ile bağlanan yerlerde veri değişikliğini anında yansıtır.
    -->

    <div id="app" style="text-align:center;">

        <div id="book">
            <h1>{{title}}</h1>
            <p>{{description}}</p>
        </div>

        <div id="book-photo">
            <!-- 
                v-bind direktifini : operatörü ile daha kısa şekilde de kullanabiliriz.
                Yani v-bind:src yerine :src yazılabilir.
            -->
            <img :src="coverPhoto" v-bind:altText="alternativeText" />
        </div>

    </div>

    <script type="text/javascript">

        var app = new Vue(
            {
                el: '#app',
                data: {
                    title: "Rust Programming Cookbook",
                    description: "Perfect book about programming with rust",
                    coverPhoto: "./images/book_1.jpeg",
                    alternativeText: "Rust Programming Cookbook Cover Photo"
                }
            }
        )

    </script>
</body>

</html>
```

Sayfadaki img elementinin kullandığı resmi kaynağı ve açıklama kısmı Vue bileşeninin data özelliğinden beslenir. Yine F12 Debug moddayken bu içeriklerin değişmesi anında elementlere de yansıyacaktır. Aşağıdaki ekran görüntülerinde olduğu gibi;)

![image.axd](images/image.axd)

ve F12 Debug mod durumu.

![image.axd](images/image.axd)

Buraya kadar az çok bir Vue bileşeninin HTML DOM nesneleri ile nasıl konuştuğunu anladık diyebiliriz. Üçüncü adımımızda akış kontrol ifadelerinden if...else kullanımına bakalım.

```bash
touch vue_conditional_render.html
```

Kodlarımızı da aşağıdaki gibi geliştirelim.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 03</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>
    <!--
        02nci örneğin aynısı ancak bu kez kitabın stoktaki miktarına göre HTML elementlerinin Render edilip edilmeyeceklerini belirliyoruz.
        v-if v-else, v-else-if, v-show gibi direktiflerle HTML elementlerinin Render operasyonları koşula bağlanabilir.
        stock-state isimli div içerisinde p elementleri, quantity değerine göre görüntülenmektedir.
        onDiscount, true veya false değer almaktadır. Bu gibi sıklıkla kapalı veya açık konuma geçecek bir element söz konusu olduğunda v-show direktifinin kullanılması önerilir.
        Pek tabii Vue tarafında da switch yapısı mevcuttur.
        Yine tarayıcı Console'unda onDiscount ve quantity değerleri ile oynayarak sayfanın nasıl değişikliklere uğradığını inceleyebilirsiniz.
    -->

    <div id="app" style="text-align:center;">

        <div id="book">
            <h1>{{title}}</h1>
            <p>{{description}}</p>
            <div id="stock-state" style="font-weight:bold;">
                <p v-if="quantity>100">Depoda yeterli miktarda var. Tam {{quantity}} adet. Sakin!</p>
                <p v-else-if="quantity>50 && quantity<100">İdare ederizzzz... {{quantity}}</p>
                <p v-else-if="quantity>0 && quantity<50">Imm..Şey. Sipariş etsek mi? Sadece {{quantity}} adet kalmış</p>
                <p v-else>Ovv yooo!!! Bu da ne? {{quantity}}</p>
            </div>
            <p v-show="onDiscount">İndirimde</p>
        </div>

        <div id="book-photo">
            <img :src="coverPhoto" v-bind:altText="alternativeText" />
        </div>

    </div>

    <script type="text/javascript">

        var app = new Vue(
            {
                el: '#app',
                data: {
                    title: "Rust Programming Cookbook",
                    description: "Perfect book about programming with rust",
                    coverPhoto: "./images/book_1.jpeg",
                    alternativeText: "Rust Programming Cookbook Cover Photo",
                    quantity: 120,
                    onDiscount: true,
                    level:"Small"
                }
            }
        )

    </script>
</body>

</html>
```

Sayfada ürünün miktarına göre stock-state altındaki paragraflardan hangisinin gösterileceğine karar veriliyor. Yani verinin durumuna göre bir elementin görünümü, içeriği vs değiştirilebiliyor. İşte örneğe ait çalışma zamanı çıktıları.

![image.axd](images/image.axd)

ve F12 Console'dan quantity ile onDiscount değerlerini değiştirdikten sonraki durum.

![image.axd](images/image.axd)

Çok doğal olarak bu tip bir Vue sayfasında bileşenin kullandığı veri önemlidir. Data özelliğinin içeriği bir servisten çekilmiş bir liste olabilir. Bu durumda veriyi sayfada gösterirken basit for döngülerine ihtiyaç duyarız. Dördüncü adımda bu döngüyü bir JSON dizisi için nasıl kullanacağımızı ele alıyoruz.

```bash
touch vue_for_loop.html
```

Kodlarımızı aşağıdaki gibi geliştirelim.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 04</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>
    <!--
       app nesnesinin data özelliği ile gelen JSON içeriğinde bir liste olduğunu düşünelim. 
       Örnekte kitap kategorisindeki birkaç ürün bilgisine yer veriliyor.
       Bu listeyi sıralı bir şekilde HTML' e yazdırmak için v-for direktifinden yararlanılabilir.
       Çalışma zamanında yine Chrome Console'a girilmiş ve anlık olarak books array'indeki değerlerle oynanmıştır.
    -->

    <div id="app" style="text-align:center;">

        <div id="book">
            <h1>'{{category}}' Kategorisindeki Ürünler</h1>

            <!--
                data özelliğindeki books dizisinin herbir elemanı book olarak isimlendirilmiştir.
                {{ }} notasyonu ile book üstünden id, title, publisher ve level özelliklerine erişililir.
                Döngülerde Render edilen elementlerin tekil bir anahtar ile işaretlenmesi önerilir. 
                Bunun için :key direktifi kullanlır.
                Örnekte <p> elementlerinin id isimli özellik değeri ile tekil(unique) olması sağlanır.
            -->
            <div v-for="book in books" :key="book.id">
                <p>{{book.title}} <i>({{book.publisher}}) - ${{book.listPrice}}</i></p>
            </div>

        </div>

    </div>

    <script type="text/javascript">

        /*
            data özelliği bir JSON nesnesi alabildiğinden içerisinde n elemanlı array'ler de barındırabilir.
        */
        var app = new Vue(
            {
                el: '#app',
                data: {
                    category: "Kitap",
                    books: [
                        { id: 1, title: "Programming C# for Beginners", publisher: "Wrox", listPrice: 19.95, level: 100 },
                        { id: 2, title: "Patterns of Enterprise Application Architecture", publisher: "Addison Wesley", listPrice: 34.50, level: 300 },
                        { id: 3, title: "Game Engine Architecture", publisher: "Gregory", listPrice: 45.50, level: 300 },
                    ]
                }
            }
        )

    </script>
</body>

</html>
```

Bu adımdan sonraki çalışma zamanı çıktıları ise aşağıdaki gibi olacaktır.

![image.axd](images/image.axd)

ve F12 ile Console'a geçip dizinin elemanlarında değişiklik yaptıktan sonrası.

![image.axd](images/image.axd)

Bir Web sayfası mutlaka kullanıcı ile etkileşim halindedir. Dolayısıyla sayfa üstünde gerçekleştireceği bazı olayların Vue bileşeni tarafında da ele alınması gerekir. Beşinci adımda bunu anlamaya çalışacağız.

```bash
touch vue_event_handling.html
```

HTML sayfasının kodları da şöyle.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 05</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>
    <!--
        Sayfadaki olaylar v-on direktifi kontrollere bağlanabilir.
        Örnekte kitap fiyatını artırmak ve azaltmak için iki button ve click olayları kullanılmaktadır.
    -->

    <div id="app" style="text-align:center;">

        <h1>'{{category}}' Kategorisindeki Ürünler</h1>

        <div v-for="book in books" :key="book.id">
            <p>
                <!-- 
                    CSS stillerini de veriye bağlayabiliriz.
                    span elementinin arkaplan rengini belirleyen backgrounColor değeri o anki book nesnesinin color özelliğine bağlanmıştır.
                -->
                <span :style="{backgroundColor: book.color}">     </span>
                {{book.title}}
                <i>({{book.publisher}}) - ${{book.listPrice}}</i>
                <!--
                    Fiyat artırma işini üstlenen click olayı gerçekleştiğinde ifade içerisindeki kod çalışır. Bulunulan book nesnesinin listPrice değeri 1 artar.
                -->
                <button v-on:click="book.listPrice+=1">+</button>
                <!--
                    Ancak olayları aşağıdaki gibi fonksiyonlara devrederek kullanmak daha doğrudur.
                    Bu kez button click olayı gerçekleştiğinde book.id değerini alan decreasePrice metodu çağrılır.
                    Bu metod Vue nesnesinin opsiyonel parametrelerinden olan methods içerisinde tanımlanır.
                    v-on direktifi aşağıdaki gibi @ ifadesi ile daha kısa şekilde yazılabilir.
                    Örnekte disabled niteliği de indirim yapılıp yapılmayacağını belirten incAvailable düğmesine bağlanmıştır.
                    Mesela 2 numaralı ürün için indirim uygulanamaz.
                -->
                <button @click="decreasePrice(book.id)" :disabled="!book.incAvailable">-</button>
            </p>
        </div>

    </div>

    <script type="text/javascript">

        var app = new Vue(
            {
                el: '#app',
                data: {
                    category: "Kitap",
                    books: [
                        { id: 0, title: "Programming C# for Beginners", publisher: "Wrox", listPrice: 19.95, level: 100, incAvailable: true, color: "blue" },
                        { id: 1, title: "Patterns of Enterprise Application Architecture", publisher: "Addison Wesley", listPrice: 34.50, level: 300, incAvailable: true, color: "red" },
                        { id: 2, title: "Game Engine Architecture", publisher: "Gregory", listPrice: 45.50, level: 300, incAvailable: false, color: "red" },
                    ]
                },
                methods: {
                    /*
                        button click olayı gerçekleştiğinde çalıştırılan metot.
                    */
                    decreasePrice(id) {
                        console.log(id, ". eleman için 1 dolar indirim");
                        this.books[id].listPrice -= 1;
                    }
                }
            }
        )

    </script>
</body>

</html>
```

Kullanıcı kitap fiyatlarını artırıp azaltabilir. Her iki aksiyon için olay bildirimlerinin nasıl yapıldığına dikkat edin. Olayın gerçekleşmesi sonucu çalışacak kod bir direktif ile birlikte yazılabileceği gibi Vue bileşeninin methods özelliği içerisinde de konuşlandırılabilir.

![image.axd](images/image.axd)

Yine F12 - Console penceresinde CSS rengini değiştirecek şekilde veriyle oynayabiliriz.

![image.axd](images/image.axd)

Altıncı adımda verinin HTML elementlerinin içeriğine göre bir hesaplamaya dahil edilmesine bakacağız. Burada Vue bileşeninin computed özelliğindeki fonksiyonlar devreye giriyor. Listelenen kitaplardan herhangi birinin üstüne gelindiğinde o kitabın fiyatı güncel döviz kuru değerine göre hesaplatılıp alt tarafta yazılıyor. Burada senaroyu biraz daha zengileştirebilirsiniz. Örneğin fare imleci kitap adının üstüne geldiğinde bir popup içinde resmini ve açıklamasını gösterebilirsiniz.

```bash
touch vue_computed_props.html
```

Kodlarımızı aşağıdaki gibi geliştirelim.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 06</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>

    <div id="app" style="text-align:center;">

        <h1>'{{category}}' Kategorisindeki Ürünler</h1>
        <!--
            book_count, bir computed property'dir.
        -->
        <p><i>{{book_count}} adedi satışta</i></p>

        <!--
            div üzerinde mouse bir kitaba denk geldiğinde bu kitabın indis değeri markIndex isimli fonksiyona gönderilir.
            markIndex fonksiyonu gelen bu indis değerini data elementindeki selectedBookIndex'e set eder.
            Buna göre computed içerisinde hangi kitap için işlem yapacağımızı anlayabiliriz.
        -->
        <div v-for="(book,index) in books" :key="book.id" @mouseover="markIndex(index)">
            <p>
                {{book.title}} <i>${{book.listPrice}}</i>
            </p>
        </div>
        <h3>Güncel kurdan fiyatı {{local_price}} liradır.</h3>

    </div>

    <script type="text/javascript">

        /*
            Computed Properties.
            app nesnesinin computed özelliğinde local_price isimli bir fonksiyon bulunmaktadır.
            Bu fonksiyon selectedBookIndex değerine işaret eden ürünün liste fiyatını güncel döviz kuru ile çarpıp geriye döndürür.
            Döndürülen değer HTML içerisinde for döngüsünün dışındaki bir h3 elementine basılır.
        */
        var app = new Vue(
            {
                el: '#app',
                data: {
                    category: "Kitap",
                    books: [
                        { id: 0, title: "Programming C# for Beginners", publisher: "Wrox", listPrice: 19.95, onSale: true },
                        { id: 1, title: "Patterns of Enterprise Application Architecture", publisher: "Addison Wesley", listPrice: 34.50, onSale: true },
                        { id: 2, title: "Game Engine Architecture", publisher: "Gregory", listPrice: 45.50, onSale: false },
                    ],
                    selectedBookIndex: 0,
                },
                methods: {
                    markIndex(index) {
                        this.selectedBookIndex = index;
                        //console.log(index);
                    }
                },
                computed: {
                    /*
                        Computed Property'ler hesaplamaya dahil ettikleri veriler değişmediği sürece cache üzerinde tutulurlar.
                        local_price, mouseover hareketine göre üzerinde durulan kitabı alıp liste fiyatını bir işlemden geçirir ve geriye bir değer döner.
                        book_count ise satışta olan kitapların adedini bulur ve geriye döner. 
                    */
                    local_price() {
                        //console.log(this.selectedBookIndex);
                        return this.books[this.selectedBookIndex].listPrice * 8.15;
                    },
                    book_count() {
                        count = 0;
                        for (i = 0; i < this.books.length; i++) {
                            if (this.books[i].onSale)
                                count++;
                        }
                        return count;
                    }
                }
            }
        )

    </script>
</body>

</html>
```

Bu örneğin çalışma zamanı çıktısı ise aşağıdaki gibi olacaktır.

![image.axd](images/image.axd)

Buraya kadarki adımlarda hep ana Vue bileşeni ile çalıştık. Çok doğal olarak HTML DOM yapısının birden fazla Vue bileşeni ile çalışması da istenebilir. Nitekim bir süre sonra ana bileşen çok fazla kalabalıklaşır. Yedinci adımda bu durumu anlamaya çalışacağız.

```bash
touch vue_components.html
```

Örnekte yer alan sportnews ve book iki ayrı Vue bileşeni olarak tasarlanmıştır. Template olarak birer div döndürdüklerine dikkat edelim. HTML'de konuşlandırılan aynı isimli elementler içerisine bu şablonlar basılır. book bileşenindeki iLikeIt olayının kullanımı da önemli. book bir alt bileşen olarak üzerinde gerçekleşen olay sonrası app isimli ana bileşeni de uyarmaktadır. Yani alt bileşene ait bir olay tetiklendiğinde üst bileşende de bir olay tetiklenmesini sağlayabiliriz. Dolayısıyla bileşenler birbirleriyle olaylar üzerinden de iletişim kurabilirler.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 07</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>

    <div id="app" style="text-align:left;color:white">
        <sportnews></sportnews>
        <!--
            Bileşenleri aşağıdaki gibi de kullanabiliriz. book isimli bileşenin verisi app bileşenindeki data içerisinde yer alan json dizisidir.
            book elementi içerisinde :property_name şeklindeki tanımlamalar ile döngünün dolaştığı book nesnesinin değerleri bileşen içerisine aktarılır.
            :property_name bilgileri book bileşeninin props özelliğinde tanımlanmıştır.
            
            Ek: Alt bileşenden üst bileşene bildirim yollamak.
            Amaç bir kitabın "Beğendim" butonuna basıldığında app bileşenindeki(üst component) toplam beğeni sayısını artırmak.
            Bunun için book bileşeninde düğmeye basıldığında, üst bileşene bunu bildirecek şekilde bir mesaj göndermek gerekir.
            Bu mesaj $emit fonksiyonu ile yollanabilir(book içerisindeki iLikeIt metoduna bakın)
            @i-like-it niteliğinde belirtilen updateLikeCount fonksiyonu ise alt bileşenlerden birisi i-like-it mesajını yukarı fırlattığında çağırılır.
            Bu arada hangi book bileşenine basıldığını anlamak için (b,index) çiftindeki index değerini updateLikeCount metoduna parametre olarak verebiliriz.
        -->
        <div v-for="(b,index) in books" :key="b.id">
            <book :book_title="b.title" :book_summary="b.description" :book_authors="b.authors"
                :book_list_price="b.listPrice" @i-like-it="updateLikeCount(index)">
            </book>
        </div>
        <p style="color: purple;">Toplamda {{likeCount}} kere beğen düğmelerine bastınız!</p>
    </div>

    <script type="text/javascript">
        /*
            Bu örneğe kadar dikkat edileceği üzere app nesnesinin data, computed, methods gibi özelliklerinin kalabalıklaşmaya başladığını gördük.
            Yönetilebilir ve modüler yapıdaki bir Vue sayfası için bileşenler(component) kullanmak doğru bir yaklaşımdır.
            Yani ana sayfadaki component'in alt bileşenlerden oluştuğunu düşünebiliriz.
            Örnekte iki component tanımlanmış ve app isimli div içerisinde kullanılmıştır.
            
            sportnews isimli bileşen oldukça sıradandır. Kendi verisini kullanır.
            book isimli bileşen ise app bileşenindeki data içeriğini kullanır.
            
            Bileşenler component fonksiyonu ile tanımlanır.
            Her bileşen bir template kullanmalıdır. 
            template özelliği bir container döndürmelidir(div gibi)
        */
        Vue.component('sportnews', {
            template:
                `
                <div class='sportnews' style='text-align:left;backgroundColor:purple;'>
                    <h2>Günün Öne Çıkan Spor Haberi</h2>
                    <p><h3>{{title}}</h3></p>
                    <p>{{summary}}</p>
                </div>
            `,
            data() {
                return {
                    title: "Shane Larkin Milli Takıma Çağırıldı",
                    summary: "Bir süredir ülkesi ABD'de olan Shane Larkin, Anadolu Efes kampına döndükten sonra doğrudan milli takıma çağırıldı."
                }
            }
        });

        Vue.component('book', {
            template:
                `
                <div class='book' style='text-align:right;backgroundColor:gold;color:purple'>
                    <p><h3>{{book_title}}</h3></p>
                    <p>{{book_summary}}, {{book_authors}}<br/>{{book_list_price}} TL</p>
                    <button v-on:click="iLikeIt">Beğendim</button>
                </div>
            `,
            methods: {
                iLikeIt() {
                    console.log('book bileşeninin iLikeIt olayı çağrıldı');
                    /*
                        $emit ile button click olayı tetiklendiğinde üst bileşene i-like-it olayı gerçekleşti şeklinde bir bilgi yollanır.
                    */
                    this.$emit('i-like-it');
                }
            },
            props: {
                book_title: {
                    type: String,
                    required: true
                },
                book_summary: {
                    type: String,
                    required: true
                },
                book_authors: {
                    type: String,
                    required: true
                },
                book_list_price: {
                    type: Number,
                    required: true
                },
            }
        });

        var app = new Vue(
            {
                el: '#app',
                data: {
                    likeCount: 0, // Alt bileşenlerdeki düğmeye basıldığında bu değeri artırıyoruz
                    books: [
                        {
                            id:1001,
                            title: "Veba",
                            description: "Camus adı çoğu okur için Yabancı romanıyla özdeşleşir. Ancak yazarın en önemli yapıtı aslında Veba'dır...",
                            authors: "Albert Camus",
                            listPrice: 34
                        },
                        {
                            id:1002,
                            title: "Mahur Beste",
                            description: "Mahur Beste'de Tanpınar'ın Huzur ve Sahnenin Dışındakiler adlı romanlarında önemli bir motif olan 'Mahur Beste' teması önemli yer tutar. Mahur Beste, acı bir aşk hikayesinin klasik musiki kalıplarıyla soyutlanmasıdır...",
                            authors: "Ahmet Hamdi Tanpınar",
                            listPrice: 23
                        },
                        {
                            id:1003,
                            title: "1Q84",
                            description: "Sarsıcı bir yolculuğa hazır mısınız? Öyleyse kemerlerinizi bağlayın. Erkekleri, titizlikle geliştirdiği bir yöntemle öteki dünyaya gönderen genç bir kadınla tanışacaksınız. Ve amansız bir takiple onun peşine düşen fanatik bir cemaatin müritleriyle…",
                            authors: "Haruki Murakami",
                            listPrice: 23
                        },
                        {
                            id:1004,
                            title: "Beden Kayıt Tutar",
                            description: "Ne yazık ki şimdiki psikiyatri anlayışı, yakınmalarınızı anlatmanız ve hekimin de bu yakınmaları düzeltecek bir ilaç önermesi üzerine kurulu. Ancak 'Hiç bir ilaç, kötü geçmiş bir çocukluğu düzeltmiyor'...",
                            authors: "Bessel A. van der Kolk",
                            listPrice: 41,
                        }
                    ]
                },
                methods: {
                    /*
                        Alt bileşenin emit ile gönderdiği mesaj sonrası tetiklenen metot
                    */
                    updateLikeCount(index) {
                        selected_title = this.books[index].title;
                        console.log("`", selected_title, "` isimli kitabı beğendin");
                        console.log('Üst bileşen(app) için updateLikeCount olayı çağırıldı');
                        this.likeCount += 1;
                    }
                }
            }
        );

    </script>
</body>

</html>
```

Bu örneğe ait çıktıları aşağıda görebilirsiniz. Gözleriniz kanayabilir o nedenle güneş gözlüğü kullanmanızı ya da monitörden beş metre kadar uzaklaşıp kısık gözle bakmanızı rica ederim:D

![image.axd](images/image.axd)

Yine F12 - Console üstünde oynayıp veri değişimlerini izlemekte yarar var.

Bu tip kullanıcı etkileşimli sayfalarda bir diğer konu ise Form kullanımıdır. Yani Form verisi ile Vue tarafı nasıl haberleşebilir, POST edilen veri nasıl ele alınabilir sekizince ve son adımda bunu anlamaya çalışacağız.

```bash
touch vue_forms.html
```

Son sayfamıza ait kodları aşağıdaki gibi yazabiliriz.

```text
<html>

<head>
    <title>VueJs Bebek Adımları - 08</title>
    <!-- CDN adresinden Vue.js'i kullanacağımızı söyledik-->
    <script src="https://unpkg.com/vue"></script>
</head>

<body>

    <div id="app" style="text-align:left">

        <!--
            @new-book-created, book-form bileşeninin onSubmit olayı içerisinden yapılan bildirimin adıdır.
            Bu bildirim gerçekleştiğinde üst bileşen yeni bir kitap üretildiğini anlayabilir ve bu nesneyi kendi data nesnesindeki books isimli diziye ekleyebilir.
            Bunun için addBookToList metodu kullanılır.
        -->
        <book-form @new-book-created="addBookToList"></book-form>

        <div v-for="(b,index) in books" :key="b.id">
            <p>
            <h2>{{b.title}} ({{b.like}} beğeni)</h2>
            </p>
            <p>{{b.summary}}</p>
        </div>

    </div>

    <script type="text/javascript">

        /*
            Örnekte bir form kullanılarak Submit işlemi ele alınıyor.
            input, textarea, select gibi girdi elemanları v-model direktifi yardımıyla data fonksiyonundan döndürülen alanlara bağlanırlar.
            select kontrolünde kullanılan .number, option içeriğinin integer olarak dönüştürülmesini sağlar.
            Submit işlemi gerçekleştiğinde @submit.prevent ile belirtilen onSubmit metodu tetiklenir.
            Form Validation için onSubmit metodunda bir takım tedbirler aldık. 
            Bu arada HTML 5 için required ifadesi ile elementlerin zorunlu hale getirilebileceğini de belirtelim.
        */
        Vue.component('book-form', {
            template:
                `
                <div class='book'>
                    <form class="new-book-form" @submit.prevent="onSubmit">
                        <p>
                            <label for="title">Kitabın adı nedir?</label>
                            <input id="title" v-model="title" placeholder="title">
                        </p>                    
                        <p>
                            <label for="summary">Düşüncelerin neler?</label>      
                            <textarea id="summary" v-model="summary"></textarea>
                        </p>                        
                        <p>
                            <label for="like">Ne kadar beğendin?</label>                        
                            <select id="like" v-model.number="like">
                                <option>1</option>
                                <option>2</option>
                                <option>3</option>
                                <option>4</option>
                                <option>5</option>
                            </select>
                        </p>                            
                        <p>
                            <input type="submit" value="Ekle">  
                        </p>            
                        <p v-if="errors.length" style="color:red;">
                            <b>Hata : </b>
                            <ul>
                                <li v-for="err in errors">{{ err }}</li>
                            </ul>
                        </p>            
                    </form>                    
                </div>
            `,
            data() {
                return {
                    title: null,
                    summary: null,
                    like: null,
                    errors: []  // Form doğrulama hatalarını tutmak için eklendi
                }
            },
            methods: {
                /* 
                    Submit düğmesine basılınca tetiklenir.
                    this ile bu bileşenden gelen name, summary, like gibi alanlar ele alınabilir.
                    Bu değerler kullanılarak yeni bir nesne oluşturulur.
                */
                onSubmit() {
                    this.errors = [];
                    if (this.title && this.summary) {
                        let newBook = {
                            title: this.title,
                            summary: this.summary,
                            like: this.like
                        }
                        /*
                            Üst bileşene yeni bir girdi oluşturulduğuna dair bilgiyi yine $emit ile gönderebiliriz.
                            İkinci parametre ile oluşturulan nesne örneği üst bileşene yollanır.
                        */
                        this.$emit('new-book-created', newBook)
                        /*
                            newBook örneklendikten sonra bu bileşenin verisi temizlenir ve yeni veri girişine uygun hale getirilir.
                        */
                        this.title = null
                        this.summary = null
                        this.like = null
                    } else {
                        /*
                            Doğrulama için koyduğumuz kısım.
                            Eğer başlık veya özet girilmemişse bunla ilgili olarak bu bileşenin errors dizisine bilgi ekliyoruz.
                        */
                        if (!this.title) this.errors.push("Kitap başlığı girilmeli.")
                        if (!this.summary) this.errors.push("Kitap için geri bildirim eklenmeli.")
                    }
                }
            }
        });

        var app = new Vue(
            {
                el: '#app',
                data: {
                    books: []
                },
                methods: {
                    addBookToList(book) {
                        /*
                            book-form bileşeninde Submit işlemi ile bir eleman eklendiğinde @new-book-created bildirimine göre bu metot çağrılır.
                            book parametresi ile gelen nesne, push fonksiyonu ile books dizisine eklenir.
                        */
                        this.books.push(book)
                    }
                }
            }
        );

    </script>
</body>

</html>
```

İlk denemelere ait bir ekran çıktısını aşağıda bulabilirsiniz.

![image.axd](images/image.axd)

Doğrulama ile ilgili kodların çıktısı da şöyle.

![image.axd](images/image.axd)

![image.axd](images/image.axd)

Örnekte ekrandan girilen kitap bilgileri books isimli JSON dizisine ekleniyor. Tahmin edileceği üzere bu veri erişimi bir servise doğru yapılmalı. Yani uygulamanın bir servis üzerinden bir veritabanı ile konuşması daha doğru olacaktır. Burada veritabanı hayal gücünüze kalmış. Senaryoya uygun bir NoSQL veya ilişkisel veritabanı kullanılabilir.

Sekizinci örnekle birlikte Vue'nun en temel parçalarını bebek adımları ile biraz olsun incelemiş olduk. Konuyu kendi kendinize çalışabileceğinizi düşünerek sizlere birkaç soru ve ödev bırakıyorum.

## Bomba Sorular

- Vue'da v-switch direktifi var mıdır? Yoksa bile kullanmanın bir yolu olabilir mi?
- vue_event_handling örneğinde tek bir karakter ekleyerek oluşacak bug'ı bulun.
- Vue.component ile bileşen tanımlanırken computed, methods özelliklerini kullanabilir miyiz?
- vue_components.html örneğinde yer alan data neden bir fonksiyon şeklinde tanımlanmıştır?
- "Props'lar üst bileşenden alt bileşene veri aktarımında kullanılırlar" ifadesi doğru mudur?

## ve Ödevler

- vue_attribute_binding.html örneğinde kitap fotoğrafına bir link bağlayın (a href) ve href niteliğinin data nesnesindeki url isimli bir özellikten beslenmesini sağlayın.
- vue_conditional_render.html örneğinde, level değişkeninin Small, Medium, Large, XLarge olmasına göre sayfanın sağ üst köşesinde S,M,L,XL harflerinin şöyle janjanlı imajlar şeklinde görünmesini sağlayın.
- vue_for_loop örneğinde yer alan level değerini kullanarak kitap fontlarını renklendirmeyi deneyin. 100 için farklı bir renk, 300 için farklı bir renk vb
- vue_event_handling örneğinde fiyat azaltmada 0 ve eksi değere geçilmesini önleyin. Ayrıca her ürün fiyatı için bir üst artırma limiti olsun ve artışlar bu değeri geçemesin.
- Vue antrenmanı yaptığınız herhangi bir sayfada yine ürünleri listeleyin ancak bir ürün adının üstüne geldiğinizde ürünün fotoğrafının olduğu bir div elementini aktif hale getirin. Yani ürün adı üstüne gelince fotoğraf gösterilmesini sağlayın (Popup ile uğraşmayın, sayfadaki bir div alanı görünür hale gelsin yeterli)
- Okduğunuz son beş kitabın sadece başlıklarını listeleyen bir bileşen tasarlayın. Bu bileşende her başlık yanında "Detay" isimli bir Button olsun. Bu düğmeye basınca kitapla ilgili detayları içeren başka bir bileşen başlığın hemen altında görünür olsun.
- Vue_forms.html örneğinde kitap ekledikçe bunu ekrana listeleyen bir bileşeni for döngüsü yardımıyla kullanmayı deneyiniz.
- Vue_forms.html örneğinde summary için maksimum 250 karakter girilmesine izin veren bir doğrulama fonksiyonelliği geliştirin.

Örnek kodlara [github reposu üzerinden](https://github.com/buraksenyurt/skynet/tree/master/No%2041%20-%20Vueeeee) erişebilirsiniz. Böylece geldik bir SkyNet derlememizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
