---
layout: post
title: "Hamming Distance Algoritmasının Basit Kullanımı"
date: 2016-01-13 17:09:00 +0300
categories:
  - csharp
  - ruby
tags:
  - csharp
  - ruby-lang
  - algoritma
  - levenshtein-distance
  - hamming-distance
  - extension-methods
  - language-integrated-query
  - richard-hamming
---
Geçtiğimiz günlerde uzun süredir görüşmediğim bir arkadaşımdan mesaj aldım. Bir projesinde Levenshtein Distance algoritmasını kullanmaya karar verdiğini ve internette arama yaparken daha önceden yazdığım [şu](/2012/07/01/levenshtein-distance-algoritmasi/) makaleye rastladığını dile getirdi. Hem kafasına takılan bir konuyu dile getirmek hem de bir hal hatır sormak istediğini belirtti.

![hamming_0.gif](/assets/images/2016/hamming_0.gif)

Yazıyı yazalı epey zaman olduğundan konuyu tamamen unutmuştum. Şöyle arkama yaslandım ve bir güzel yazdıklarımı okudum. Tabii dolu imla hatası buldum ama neyseki algoritmanın kullanım amacını yeniden hatırlamayı başardım. Derken Levenshetin Distance gibi benzer fark bulma algoritmaları olup olmadığına bakmaya karar verdim. Bu iş için geliştirilmiş bir çok algoritma vardı. Derken kendimi Hamming Distance algoritmasını incelerken buldum.

Hamming Distance, Amerikalı Matematikçi [Richard Hamming](https://tr.wikipedia.org/wiki/Richard_Hamming) tarafından bulunmuş olan ve kodlama teorisinde geçen vektör bazlı bir karşılaştırma algoritmasıdır ([Bu adresten teori hakkında biraz bilgi alabiliriz](http://www.maths.manchester.ac.uk/~pas/code/notes/part2.pdf)) Programlama tarafından baktığımızda çoğunlukla eşit uzunluktaki içeriklerin benzerliklerine ilişkin bir mesafe ölçüsünün bulunmasında kullanılır. Bu sayede bir metnin diğerine dönüştürülebilmesi için kaç adımlık değişime ihitiyaç duyulduğu da hesaplanabilir. Ya da benzerliğin ne kadarlık bir değere denk geldiği anlaşılabilir. Hata tespiti ve düzeltilmesi, grafik dosyaları üzerinden şekil eşleştirmelerinin (Shape Recognation) yapılması gibi hesaplamalarda kullanılmaktadır. Aslında örnekler ile konuyu daha iyi anlayabiliriz. Basit düşünmeye çalışalım ve aşağıdaki gibi bir kaç kelime çiftini ele alalım.

kuru - duru
clone - drone
patates - domates
dolu - kedi

Öncelikle karşılaştırılan tüm kelimelerin birbirleri ile eşit uzunlukta olduklarını söylememiz lazım. Bu zaten algoritmanın şartlarından da birisi. kuru ve duru arasında sadece bir harflik fark var. Buna göre Hamming Distance değeri 1. Nitekim k ve d dışındaki harfler aynılar. clone ve drone karşılaştırmasına göre aradaki fark değeri ise 2dir. Yani iki harflik bir farklılık vardır. c,l ve d,r harfleri. patates ve domates'e gelince. Buradaki Hamming değeri 3tür. p,a,t ve d,o,m harfleri farklıdır. Son eşleşmeye baktığımızda ise bütün harflerin tamamen farklı olduğunu görebiliriz. Buna göre Hamming Distance değeri 4tür. Hatta bu değer kelimedeki harf sayısı kadar olduğundan her iki eşin birbirlerinden tamamen farklı olduğu sonucuna da varabiliriz. Aşağıdaki grafik ile olayı özetleyelim.

![HammingMini.gif](/assets/images/2016/HammingMini.gif)

> Tabii binary içerikler için algoritmanın çalıştırılması biraz daha farklıdır. İkili (Binary) sayı sistemi söz konusu olduğunda Hamming Distance değerini bulmak için XOR operatöründen yararlanılır.

Kod tarafında Hamming Distance algoritmasını uygulamak son derece kolay..Net tarafında LINQ kabiliyetlerini kullanabiliyoruz. Hatta Ruby'de bile oldukça basit. Gelin her iki dil için Hamming Distance hesaplaması yapan birer örnek geliştirelim. (Malumunuz ben bir Rubyist olmaya gayret ediyorum. Ona yer vermezsem olmazdı)

## C# Örneğimiz

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace HammingDistance
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> words = new Dictionary<string, string>
            {
                { "kuru","duru" }
                , {"clone","drone" }
                , {"patates" ,"domates"}
                , {"silindir","bilindik" }
                , {"tabela","tabela" }
                , {"sempatik","sentetik" }
                , {"eksik","balık" }
                , {"dolu","kedi" }
            };
            int distanceValue;
            foreach (var pair in words)
            {
                distanceValue=pair.Key.CalculateHammingDistance(pair.Value);
                Console.WriteLine("{0} vs {1}\tiçin : {2} ",pair.Key,pair.Value,distanceValue);
            }
        }
    }

    public static class StringExtensions
    {
        public static int CalculateHammingDistance(this string source, string target)
        {
            if (source.Length != target.Length)
                throw new Exception("Metinler eşit uzunlukta olmalı");

            int distance =
                source.ToCharArray()
                .Zip(target.ToCharArray(), (char1, char2) => new { char1, char2 })
                .Count(m => m.char1 != m.char2);

            return distance;
        }
    }
}
```

Örneğin çalışma zamanı çıktısı aşağıdaki gibidir.

![Hamming_1.gif](/assets/images/2016/Hamming_1.gif)

Kodda basitçe neler yaptığımıza bir bakalım. String tipi için yazılmış bir genişletme metodu (Extension Methods) olduğunu görmüşsünüzdür. CalculateHammingDistance metodu bir string değişkene uygulanabilir. target isimli değişken ile gelen içerik source ile karşılaştırılmaktadır. LINQ ifadesindeki dikkat çekici nokta ise Zip metodudur. Sanırım pek çoğumuz bu fonksiyon ile ilk kez karşılaşıyor. Zip fonksiyonu iki diziyi istenen bir ifade çerçevesinde (Predication diyelim) birleştirmek amacıyla kullanılmaktadır. Öncesinde dikkat edileceği gibi source içeriği bir karakter dizisine çevrilir. Zip fonksiyonu target değişkeninin taşıdığı karakter dizisi ile source içeriğine ait karakter dizisini yeni bir isimsiz tip (Anonymous Type) altında birleştirir. Son olarak da bu dizi elemanlarının birbirleri ile aynı olup olmadığı kontrolü yapılır. Count metodunda yer alan m değişkeni new {char1, char2} ifadesi ile üretilen nesne örneklerini ifade eder.

## Ruby Örneğimiz

Ruby tarafında da oldukça benzer bir yaklaşım söz konusudur. Hatta metod adları neredeyse aynıdır diyebiliriz. İşte Ruby kod parçacığımız.

```bash
#Ruby icin Hamming Distance

class StringOperations

def calculateHammingDistance(source, target)
raise "ERROR: Hamming: kaynak ve hedef icerikler esit uzunlukta degiller!" if source.length != target.length
(source.chars.zip(target.chars)).count {|left, rigth| left != rigth}
end

end

words=Hash.new("Words")
words["kuru"]="duru"
words["clone"]="drone"
words["patates"]="domates"
words["silindir"]="bilindik"
words["tabela"]="tabela"
words["sempatik"]="sentetik"
words["dolu"]="kedi"

sOp=StringOperations.new
words.each{
|key,value| 
puts "#{key} vs #{value} = #{sOp.calculateHammingDistance(key,value)}"
}
```

ve çalışma zamanı çıktımız.

![Hamming_2.gif](/assets/images/2016/Hamming_2.gif)

Ruby kod örneğimizde StringOperations sınıfı içerisinde yazdığımız calculateHammingDistance isimli fonksiyonu kullanıyoruz. İki parametre alan fonksiyon source ve target içeriklerin kıyaslayarak ilgili Hamming Distance değerini döndürüyor. Metodun ilk satırında kaynak ve hedef metinlerin uzunluklarının aynı olmaması halinde bir Exception fırlatılmasını sağlıyoruz (Benzer durum C# ile yazdığımız kod örneği için de geçerli) İkinci satırda ise chars, zip ve count metod kullanımları söz konusu. LINQ ile yazdığımız ifadeye ne kadar da benzer değil mi?:) İzleyen kod satırlarında ise key:value çiflerini tutan bir Hash dizisini ele alarak Hamming değerlerini ekrana yazdırıyoruz.

Bu yazımızda eşit uzunluklu içeriklerin karşılaştırılarak birbirlerinden olan farklarının hesaplanmasında kullanılan Hamming Distance algoritmasını incelemeye çalıştık. Kullanım alanlarını keşfederek kendiniz için farklı örnekler yapmaya çalışmanızı öneririm. Hatta yatkın olduğunuz dilde bu algoritmayı kullanmayı denemeniz iyi bir pratik olacaktır. Diğer yandan algoritma geliştirmeye yeni başlayan arkadaşlarımızın LINQ ifadelerinden yararlanmadan bu hesaplamayı nasıl yaptırabileceklerine odaklanmalarını şiddetle tavsiye ederim. Böylece geldik bir makalemizin daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
