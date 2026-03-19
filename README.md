# Blog Works

İlk blog yazımı 2003 yılının bir Kasım ayında yazmıştım. Aradan geçen yıllar boyunca yazılımın farklı konularında birçok makale kaleme almaya çalıştım. Zaman içerisinde blog altyapısı, internet standartları ve kullanıcı alışkanlıkları değişti. Örneğin bugün vazgeçemediğim markdown gibi formatlar çıkıp yaygınlaştı. Sonuçta blogda hatırı sayılır derecede ve çeşitli çalışmalar için kaynak olarak kullanabileceğimiz bir içerik oluştu. Bu içeriği pekala *context* olarak ele alıp yapay zeka araçları ile kullanabiliriz. Bu repository'nin açılış amacı budur.

## Veri Seti

XML formatlında tutulan makale içerikleri **GPT 5.4** yardımıyla Markdown formatına dönüştürüldü ve insan gözüyle kontrol edilebilir hale getirildi. Dönüştürülen içerikler `resource\articles` klasöründe yer almaktadır.

## Planlar

- Blog yazılarının içeriklerinin ele alındığı bir **RAG *(Retrieval-Augmented Generation)*** kurgusu oluşturmak.
  - Buna bağlı olarak blog yazılarının içeriklerini kullanarak farklı konularda **soru-cevap** tarzında bir **chatbot** hazırlamak.
- İçerikleri kullanabilen bir API tasarlayıp bunu bir **MCP *(Model Context Protocol)*** server arkasına almak.
- İçinde tüm blog yazılarını ve yazılarda kullanılan görselleri barındıran offline bir **knowledge base** oluşturmak.

ve daha fazlası...

## Çalışma Günlüğü

[Updates](Updates.md) dosyasında yer alır.
