---
title: "NextJS ile Web API Tüketen bir React Uygulaması Geliştirmek"
pubDate: 2020-07-18 19:22:00
categories:
  - React
tags:
  - React
  - Bash
  - JavaScript
  - REST
  - JSON
  - Web API
  - HTTP
  - Async/Await
  - GitHub
---

# NextJS ile Web API Tüketen bir React Uygulaması Geliştirmek
![nextjs.png](images/nextjs.png)

Geçen sene React ile ilgili basit birkaç örnek denemiiş olmama rağmen öğrendiklerimi çok çabuk unuttuğumu fark ettim. Gerçek saha projelerinde sıklıkla kullanmayınca böyle bir şeyin olması kaçınılmazdı. Dolayısıyla [skynet çalışmaları](https://github.com/buraksenyurt/skynet) kapsamında tekrardan pratik yapmanın uygun olacağını düşündüm. Bu sefer amacımız Star Wars için [https://swapi.dev](https://swapi.dev/) adresinden sunulan REST servisini tüketen ve karakterlerin listesini gösteren bir web uygulaması geliştirmek. Bunu yaparken hafif siklet kategorisinde sayılan ancak bir çok işi kolaylaştırdığı söylenen Next.Js isimli web framework'ten faydalanacağız.

Örneğin dikkat çekici noktalarından birisi servisin karakter listesini sayfalama yoluyla vermesi. Yani tüm karakterleri tek seferde veren bir REST servis yerine, diğer sayfaları da ayrı HTTP Get çağrıları ile alacağımız bir yaklaşım söz konusu. Servis çağrısı sonrası elde edilen her JSON içeriğinde previous, next gibi önceki ve ileriki servis noktalarını referans eden nitelikler bulunduracağız. İşleyiş olarak sayfanın altındaki "Daha fazlası..." yazan düğmeye bastıkça yeni içerik var olanın arkasına eklenecek ve liste aşağı doğru uzayıp gidecek. Bu durumu önyüz tarafında yönetmek için çekilen içeriği ve sonraki servis adresi bağlantı bilgilerini tutmamız gerekiyor. İşte bu noktada useState, useEffect gibi enstrümanlarla React Hooks'u tanımaya çalışacağız. Ben örneğimizi Heimdall (Ubuntu-20.04) üstünde geliştiriyorum ancak diğer plaformlarda da benzer şekilde yazabilirsiniz. Haydi gelin idmanımıza başlayalım.

```bash
# Önce next.js destekli react uygulamasını oluşturalım (ben adını star-wars-peoples olarak isimlendirdim)
npx create-next-app

#Önyüz görünümünde Bootstrap bileşenlerini kullanabilmek için
npm install react-bootstrap bootstrap
```

Kodsal değişikliklerimiz pages altındaki index.js dosyasında yer alıyor. Burayı incelemeniz konuyu anlamak adına yeterli olacaktır. Pek tabii yorum satırları en büyük yardımcınız. Bootstrap içinse sadece _app.js düzenlenmiştir.

```javascript
import { useState, useEffect } from 'react'; //useState ve useEffect kullanımı için eklendi
import Head from 'next/head'
import styles from '../styles/Home.module.css'
import {Card,ListGroup, Button} from 'react-bootstrap'; // Bootstrap elemanlarını kullanabilmek için ekledik

const defaultUrl=`https://swapi.dev/api/people/`; // Kullanacağımız servis adresini bir sabit değişkene aldık

/*
  üstte tanımladığımızı adresi kullanarak tüm JSON verisini çeken asenkron fonksiyonumuz.
  getServerSideProps, nextjs'in veri çeken fonksiyonlarından birisidir.
  Farklı isimle kullanmaya çalıştığımızda çağırılmadığını görürürüz. 
*/
export async function getServerSideProps(){
  const response=await fetch(defaultUrl); // HTTP Get talebini gönderdik
  const peoples=await response.json(); // Çekilen veri JSON formatında olduğu için, dönüştürdük
  /*
    component üzerinde verinin kullanılabilmesi için properties içerisine ekledik. 
    Bunu Home bileşeninde kullanmak için parametre olarak eklediğimize dikkat edelim.
  */
  return{
    props:{
      peoples
    }
  }
}

export default function Home({peoples}) { //props ile gelen peoples parametre olarak eklendiği için içeride kullanılabilecek
  
  // İlk denemede verinin gelip gelmediğini tespit etmek için kullanabiliriz.
  // console.log('starwars-peoples',peoples); //F12 ile console penceresinden bakılabilir
  
  // İstediğimiz bilgiler JSON verisindeki results altında duruyor. Bunları bir diziye aldık.
  // Div kontrolünün içeriğini doldururken bir array'den yararlanıyoruz.
  // const results=[]=peoples.results; // State kullanımına geçildiği için kapatıldı
  //console.log(results);

  /*
    Sayfanın altında yer alan "Daha fazlası..." düğmesine basılınca var olan durumu korumayı ve üstüne yeni servis talebi ile
    çektiğimiz verileri de ekleyip göstermeyi istiyoruz. Bu nedenle result, sonraki sayfa, gibi verileri güncel tutacağımız state
    nesnelerini ele alacağız.
    state'lerde kullanabilmek için gerekli değişkenleri, peoples ismiyle gelen JSON içeriğinden alıyoruz.
    peoples getServerSideProps sayesinde zaten ilk yüklemede dolduruluyor.
    swapi'deki json desenine göre içinden next ile results niteliklerini alıyoruz.
  */
  const {next, results: defaultResults = [] } = peoples;

  /* 
    results isimli bir sabit tanımladık ve bunun içeriğini updateResults ile güncelleyeceğimizi söyledik.
    Varsayılan değerini de peoples isimli json nesnesinden çektiğimiz defaultResults dizisi ile doldurduk.
  */
  const [results, updateResults] = useState(defaultResults);

  /*
    Burada da page isimli bir değişken tanımladık ve
    içeriğini updatePage isimli metodla güncelleyeceğimizi söyledik.
    page'in varsayılan değerini de peoples nesnesinden yakaladığımız
    değerlerle doldurduk.
  */
  const [page, updatePage] = useState({
    next,
    current: defaultUrl
  });

  const { current } = page;

  /*
    Bileşen her render edildiğinde devreye giren useEffect metodu React lifecyle sürecindeki componentDidMount, componentDidUpdate ve componentWillUnmount fonksiyonlarının görevini üstlenmekte.
    
    Bileşen ilk yüklendiğinde loadMore olay tetiklenmesi gerçekleştiğinde devreye giriyor.
  */
    useEffect(() => {

    // İlk açılış sayfasındaysak varsayılan servis linki geçerlidir. 
    // Diğer yandan json'daki next bilgisi null ise son sayfaya gelmişizdir
    // Bu iki halde return edilir.
    if ( page.current === defaultUrl || !current) return;
    
    //Bir request çağrısı aldığımızda (Örneğin loadMore tetiklendiğinde)
    async function request() {
      // current ile saklanan adres ne ise oradan veri çekiyoruz
      const res = await fetch(current)
      const data = await res.json();

      // console.log('Gelen içerik->',data.results);
      // console.log('data.next->',data.next);
      // console.log('data.previous->',data.previous);
      // console.log('current page->',page.current);

      //Güncellenen state değişken verisini yenileri ile dolduruyoruz. updatePage, page sabiti ile ilintiliydi 
      updatePage({
        next:data.next,
        current,
      });

      // Eğer gelen json verisindeki previous değeri boşsa(yani ilk sayfadaysak) 
      if ( !data?.previous ) {
        updateResults(data.results); //data'nın sahip olduğu varsayılan veri ile dolduruyoruz
        return;
      }

      /*
        Hali hazırda dolu olan verinin üstüne yeni servis verisini de ekliyoruz.
        Liste aşağıya doğru uzayıp gidecektir böylece. Bunu sağlarken preData ile güncel data.results verisini üst üste ekledik.
        data.results yeni servis çağrısı ile gelen veri, preData ise koruduğumuz veri.
      */
      updateResults(preData => {
        return [
          ...preData,
          ...data.results
        ]
      });
    }

    request();

  }, [current]);

 function loadMore() {
   /*
    Düğmeye basılınca bu fonksiyon çalışıyor.
    Fonksiyon updatePage yardımıyla page sabitinin içeriğini güncelliyor.
   */
   updatePage(prePage => {
     //console.log("pre nedir?->",prePage);
    return {
      prePage,
       current: page?.next //Sonraki servis adresi verisini aldık
     }
   });
 }

  return (
    <div className={styles.container}>
      <Head>
        <title>Star Wars İnsanları</title>
      </Head>

      <main className={styles.main}>
        <h1 className={styles.title}>
          Tüm Karakterler
        </h1>

        <p className={styles.description}>
          Star Wars evrenindeki tüm karaktelerin temel bilgilerini bu listede bulabilirsiniz;)
        </p>

        <ListGroup>
          {results.map(r=>{
            const {name,birth_year,height}=r;
            return(
              <Card styles={{width:'18rem'}}>
                <Card.Body>
                  <Card.Title>
                    {name}
                  </Card.Title>
                  <Card.Text>
                    {name}, {birth_year} yılında doğmuştur. Boyu {height} cm'dir.
                  </Card.Text>
                </Card.Body>
              </Card>
            )
          })}
        </ListGroup>

        <div>
          <Button styles="btn btn-primary" onClick={loadMore}>Daha fazlası...</Button>
        </div>
        
      </main>

      <footer className={styles.footer}>
        <a
          href="https://swapi.dev/"
          target="_blank"
          rel="noopener noreferrer"
        >Diğer API Hizmetleri için tıklayın.
        </a>
      </footer>
    </div>
  )
}
```

## Çalışma Zamanı

Index sayfasını tamamladıktan sonra terminal'den aşağıdaki komutu verip sonrasında localhost:3000 adresini ziyaret etmemiz yeterli.

```bash
npm run dev
```

Ve çalışma zamanına ait bir görüntü.

![skynet_24_Screenshot_1.png](images/skynet_24_Screenshot_1.png)

Kodları denerken yorum satırı olan console.log satırlarını açmanız işe yarayabilir. F12 Developers Tools sekmesinde bu sayede akan mesajları da görebilirsiniz. Tabii uygulamada ufak bir problemimiz de var. Minik bir bug diyelim:D "Daha fazlası..." butonuna bastıkça listemiz açılıyor ancak bir önceki konuma dönmemiz mümkün olmuyor. Söz gelimi "Azalt..." isimli bir button daha olsa ve buna basılınca state bir önceki konumuna dönse hiç fena olmaz. Sizce bunu yapmak mümkün mü? Eğer mümkün olduğunu düşünüyorsanız lütfen yorumlarda belirtip bana yardımcı olun;) Böylece geldik bir skynet derlemesinin daha sonuna. Kaynak kodlara [github reposu üzerinden](https://github.com/buraksenyurt/skynet/tree/master/No%2024%20-%20A%20Simple%20React%20App%20with%20NextJS) erişebilirsiniz. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
