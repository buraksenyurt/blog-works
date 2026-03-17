# Blog Works

İlk blog yazımı 2003 yılının bir Kasım ayında yazmıştım. Aradan geçen yıllar boyunca yazılımın farklı konularında birçok makale kaleme almaya çalıştım. Zaman içerisinde blog altyapısı, internet standartları ve kullanıcı alışkanlıkları değişti. Örneğin bugün vazgeçemediğim markdown gibi formatlar çıkıp yaygınlaştı. Sonuçta blogda hatırı sayılır derecede ve çeşitli çalışmalar için kaynak olarak kullanabileceğimiz bir içerik oluştu. Bu içeriği pekala *context* olarak ele alıp yapay zeka araçları ile kullanabiliriz. Bu repository'nin açılış amacı da bu.

## Veri Seti

Tam olarak bir veri seti diyemesem de blogdaki makalelerin içerikleri **resource/posts** klasöründe **XML** formatında yer alıyor. Orjinal blogda yer alan paylaşımları kırparak sadece yazı içerenleri ele almaya çalıştım. Hali hazırda 898 adet farklı dosya*(makale)* var. Zaman çizelgesinin neredeyse çeyrek asırdan fazla bir zaman dilimine denk geldiklerinden özellikle **content** elementinin içeriği oldukça karışık. **tag** ve **category** mantığında düzeltmeler yapmak gerekiyor. Ancak en nihayetinde kırpılmış saf bir resource kaynağımız bulunmakta.

## Şema Yapısı

Makale içeriklerine ait XML dosyalarının şema yapısı şöyle;

```xml
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<post>
  <title>Yazının başlığı</title>
  <description>Yazının kısa açıklaması</description>
  <content>Yazının HTML formatındaki içeriği</content>
  <pubDate>Yayın tarihi (YYYY-MM-DD HH:MM:SS)</pubDate>
  <slug>URL dostu kısa ad</slug>
  <tags>
    <tag>etiket1</tag>
    <tag>etiket2</tag>
  </tags>
  <categories>
    <category>kategori1</category>
  </categories>
</post>
```

## Planlar

- Blog yazılarının içeriklerinin ele alındığı bir **RAG *(Retrieval-Augmented Generation)*** kurgusu oluşturmak.
  - Buna bağlı olarak blog yazılarının içeriklerini kullanarak farklı konularda **soru-cevap** tarzında bir **chatbot** hazırlamak.
- İçerikleri kullanabilen bir API tasarlayıp bunu bir **MCP *(Model Context Protocol)*** server arkasına almak.
- XML içeriklerini markdown formatına dönüştürebilecek bir **parser** aracı geliştirmek.
- İçinde tüm blog yazılarını ve yazılarda kullanılan görselleri barındıran offline bir **knowledge base** oluşturmak.

ve daha fazlası...

## Çalışma Günlüğü

[Updates](Updates.md) dosyasında yer alır.
