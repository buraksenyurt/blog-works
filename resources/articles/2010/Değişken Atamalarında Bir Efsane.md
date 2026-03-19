---
title: "Değişken Atamalarında Bir Efsane"
pubDate: 2010-04-12 02:30:00
categories:
  - C#
tags:
  - C#
---

# Değişken Atamalarında Bir Efsane
![blg149_MythBusters.jpg](images/blg149_MythBusters.jpg)

Merhaba Arkadaşlar,

Yandaki resimde görülen kahramanları tanıyanınız var mı? Biraz düşünün isterseniz...Jamie Hyneman ve Adam Savage ikilisi tarafından sunulan ve Wikipedia'daki verilere göre Discovery Channel aracılığıyla ilk yayınını 23 Ocak 2003 tarihinde gerçekleştiren MythBusters isimli bu belgesel dizide, bilimsel metodlardan yararlanılarak bazı şehir efsanelerinin gerçeklikleri ispatlanmaya çalışılmaktadır.

Aslında film sanayisinin ne kadar çok geliştiğini ispat eden bir dizidir. Nitekim dizinin sunucuları Hollywood'un tanınan film efekti teknisyenleridir (ki bu nedenle bilimsel anlamda gayet donanımlıdırlar) ve son derece ilginç efsaneleri araştırırlar. Örneğin "Yağmurda koşan mı yoksa yürüyen mi daha çok ıslanır?", "Benzin istasyonunda cep telefonu kullanmak patlmaya neden olur mu?", "Piercing yapan insanları yıldırım çarpar mı?", "Emprise State binasının tepesinden yere serbest düşüşle bırakılan metal bir para betona saplanır mı?" ve daha nice ilginç efsaneyi bilimsel taktikler ile çözmüşlerdir ve çözmeye devam etmektedirler. Açıkçası bende evdeki afacan müsade ettiği sürece bu tip belgeselleri kaçırmamaya çalışıyorum ve size de izlemenizi şiddetle tavsiye ediyorum.

Gelelim bu dizinin bu yazımızdaki konuyla ilgisinin ne olduğuna. Her zamanki gibi önce güzel bir giriş yapalım istedim. Ama asıl mesele C# tarafında da bazı efsanelerin olabileceği. Üstelik bunların çoğundan habersisiz. Ancak Internet üzerinde siz de benim gibi yeteri kadar araştırma yaparsanız bu konulara ilişkin son derece güzel yazıların olduğunu keşfedebilirsiniz. Ben bu yazımızda değişken atamaları ile ilişkili bir konuyu incelemeye çalışacağım. Olayın çıkış noktası ise aşağıdaki kod parçamız olacak.

Vaka 1;

```csharp
using System;

namespace AssignMyth
{
    class Program
    {
        static void Main(string[] args)
        {
            double x, y, z;

            x = y = z = Math.PI;

            Console.WriteLine("x={0}\ny={1}\nz={2}",x,y,z);
        }
    }
}
```

Uygulamanın çalışma zamanı çıktısı aşağıdaki gibidir.

![blg149_Runtime1.gif](images/blg149_Runtime1.gif)

Bu kod parçasında double tipinden olan x, y ve z değişkenlerine tek satırda Pi değerinin atanması söz konusudur. Bu son derece doğaldır nitekim eşitliğin sağından başlayan bir atama sırası mevcuttur. Hatta buna göre aşağıdaki ifade de doğrudur.

x = (y = (z = Math.PI));

Nitekim parantezlerin olaya kattığı her hangibir öncelik bulunmamaktadır. Fakat aşağıdaki kod parçasını göz önüne aldığımızda eşitliğin en sağ tarafındaki değerden başlayarak en soldaki değişkene doğru yapılan atamaların her zaman sanıldığı gibi olmadığı izlenimine varmamız söz konusudur.

Vaka 2;

```csharp
using System;

namespace AssignMyth
{
    class Program
    {
        static void Main(string[] args)
        {
            object x;
            double y;
            const float z = 3.14f;

            x = y = z;

            Console.WriteLine(x.GetType().ToString());
        }
    }
}
```

Ekran çıktısı aşağıdaki gibi olacaktır.

![blg149_Runtime2.gif](images/blg149_Runtime2.gif)

Hımmm...Enteresan bir durum söz konusu sanırım.

![Surprised](images/smiley-surprised.gif)

Eşitliğin en sağında yer alan z isimli değişken aslında float tipinden tanımlanmıştır. Ardından hemen solunda yer alan double tipinden değişkene aktarılmıştır. y isimli değişken double tipindendir. Son olarak eşitliğin en solunda yer alan x isimli object tipinden değişkene bir atama yapılarak 3.14 değeri en sağdan en soldaki değişkene doğru taşınmıştır. Lakin değişkenin tipi eşitliğin en sağından en soluna kadar korunamamıştır.

![Wink](images/smiley-wink.gif)

Ekran çıktısına dikkat edilecek olursa, x değişkeni gelen değeri float tipi yerine double tipi olarak ele almıştır. Yani x=y=z atamasında en soldaki x değişkeninin tipi y'nin tipine göre belirlenmektedir. Bu durumda eşitliğin en sağındaki değişkenin tipinin en soldaki object tipine taşınmasında bir anlamda bozulma olduğunu düşünebiliriz. Konuyu biraz daha ileri götürelim ve bu kez aşağıdaki kod parçasını göz önüne alalım.

Vaka 3;

```csharp
using System;

namespace AssignMyth
{
    class Program
    {
        static void Main(string[] args)
        {
            Person burak = new Person();

            object name = burak.Name = null;
            Console.WriteLine("name null mı? {0}",name==null);
            Console.WriteLine("burak.Name null mı? {0}",burak.Name==null);
        }
    }

    public class Person
    {
        private string _name;

        public string Name
        {
            get { return _name==null?"":_name; }
            set { _name = value; }
        }
    }
}
```

Dilerseniz kodun çalışma sonrası üretilen çıktıyı görmeden önce neler olduğuna bir bakalım. Person sınıfı içerisinde Name isimli bir özellik (Property) yer almaktadır. Bu özelliğe ait get bloğunda dikkat edilecek olursa null kontrolü yapılmaktadır. Bu kontrole göre String tipinden olan name alanının değerinin null olması halinde geriye boş bir string döndürülmesi tercih edilmiştir. Aksi durumda ise name değerinin kendisi döndürülmektedir. Buna göre aslında Person tipinin Name özellliği ya boş string ya da bir içeriğe sahiptir.

Main metodu içerisindeki kod parçasına baktığımızda ise ilgi çekici nokta null değer atamasının yapıldığı satırıdr. Eşitliğin en sağında yer alan burak isimli değişkenin Name özelliğine null değer atanmaktadır. Sonrasında ise bu değer object tipinden olan name değişkenine taşınmaktadır. İzleyen iki satırda ise object tipinden olan name değişkeni ile burak nesne örneğinin Name özelliklerinin değerlerinin null olup olmadığı kontrol edilmekte ve sonuçlar ekrana yazdırılmaktadır.

Normal şartlarda düşündüğümüzde burak.Name için geriye null değer dönmesi söz konusu değildir. Nitekim get bloğunda bunun için bir kontrol yapılmaktadır. Bu durumda son satırın sonucunda false değer dönmesi beklenmektedir. Diğer yandan en soldaki name değişkenine yapılan atamaya göre de null değer yerine "" değeri taşınmış olmalıdır ki buna göre de name değerinin null olması sonucunun false dönmesi gerekmektedir. Ama uygulamayı çalıştırdığımızda sonuçların aşağıdaki gibi olduğu görülecektir.

![blg149_Runtime3.gif](images/blg149_Runtime3.gif)

Oda ne?

![Surprised](images/smiley-surprised.gif)

burak.Name== null için False değer dönmüştür ve bu beklediğimiz sonuçtur. Ancak name==null kontrolünün değeri true olarak gelmiştir. Oysaki atamaya göre name değişkenine "" değerinin gelmesi ve bu nedenle null olmaması gerekmektedir. İlginç değil mi?

Sonuç olarak bu yazıda bahsettiğimiz şekliyle gerçekleştirilen atamalarda, eşitliğin en sağındaki değerin sola doğru taşındığı efsanesinin tam olarak doğru olmadığı ispatlanmış bulunmaktadır. Nitekim ilk vakada eşitliğin en sağından soluna aynı değer başarılı bir şekilde atanmaktadır. Ancak ikinci vakaya göre aslında en soldaki değişkenin bir sağındakinin tipine büründüğü de görülmektedir. Üstelik Vaka 3' e göre en soldaki değişken en sağdan atanan değere bürünmüş ve bir sağındakini kaale bile almamıştır...Kafanız karıştı mı? Bakalım başka ne gibi efsaneler var. İlerleyen yazılarda değinmeye çalışıyor olacağım. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[AssignMyth_RC.rar (20,03 kb)](assets/AssignMyth_RC.rar)
