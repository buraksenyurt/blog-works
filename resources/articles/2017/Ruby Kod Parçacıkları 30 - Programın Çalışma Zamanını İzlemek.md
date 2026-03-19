---
title: "Ruby Kod Parçacıkları 30 - Programın Çalışma Zamanını İzlemek"
pubDate: 2017-01-03 21:43:00
categories:
  - Ruby
tags:
  - Ruby
  - HTTP
---

# Ruby Kod Parçacıkları 30 - Programın Çalışma Zamanını İzlemek
Merhaba Arkadaşlar,

Diyelim ki geliştirdiğimiz kodların çalışma zamanındaki işleyişlerini izlemek istiyoruz. Sırasıyla hangi nesneler örnekleniyor, çağırılan metodlar ve varsa sonuçları hangi aşamada icra ediliyor, devreye giren C veya block çağrıları bulunuyor mu? Bu gibi sorular aslında bir uygulamayı Monitor etmek olarak da adlandırılmakta. Büyük kod parçalarında işleyiş sıralarını takip etmek ve olası istisna durumlarında kodun hangi aşamada kalmış olduğunu görmek açısından değerli bir konu. Peki Ruby kodlarının çalışma zamanı işleyişlerini nasıl izleyebiliriz. Konu ile ilgili bir çok gem veya API olsa da gömülü olarak gelen TracePoint sınıfı bize basit anlamda izleme kabiliyetleri sunmakta. Aşağıdaki örnek kod parçasını bu anlamda ele alabiliriz.

```text
@call_depth=0
TracePoint.trace(:c_call,:call,:class,:end,:b_call){|t|
  @call_depth+=1
  puts "#{t.path} #{t.defined_class}.#{t.method_id} Line : #{t.lineno} Event : #{t.event}"
}

TracePoint.trace(:return,:b_return){|t|
  puts "#{t.path} Line :  #{t.lineno} Event : #{t.event} Value :  #{t.return_value}"
  @call_depth-=1
}

class Player
  attr_accessor :nick,:level
  
  def initialize(nick,level)
    @nick,@level=nick,level
  end
  
  def Move(location)
    "Move to #{location}"
  end
end

players=[]
players<<Player.new("master vindu",19000)
players<<Player.new("luk skay valkr",21000)
players<<Player.new("princes leya",13400)
players<<Player.new("obi van kinobi",90980)
players.each{|p|
  p.Move("istanbul")
  p.level+=10
}
```

Player tipinden bir sınıfla ilgili işlemler yapıldığını görüyoruz. nick ve level şeklinde iki nitelik içeren bu sınıfa ait 4 farklı nesne örneği bir dizi içerisinde toplanıyor. Sonrasında bu dizi üzerinden örnek kod bloğu çağırılıyor. Block içerisinde her bir oyuncu için Move operasyonu gerçekleştirilip level değerleri 10ar birim arttırılıyor. Acaba bu işleyişin arka plandaki izleri nasıl? Bunun için kodun en başında TracePoint sınıfının trace metoduna çağırılar yapıldığını görmektesiniz. trace metodu parametre olarak izlenmek istenen olay veya olaylar zincirini alıyor. Birer symbol olarak gelen bu parametrelerin farklı anlamları var. Aslında bunların her biri bir olaya karşılık geliyor. Örneğin:call ile ruby metod çağrılarının gerçekeştiği anları izleyeceğimizi belirtiyoruz.:c_call ise C fonksiyon çağrılarını işaret etmekte.:b_call sembolü tahmin edeceğiniz üzere block çağrılarını takip etmek istediğimizde kullanılıyor.:class ve:end symbol'leri ile sınıf tanımlamalarının yapıldığı ve bittiği kod anlarını izleme şansına sahibiz.

Dikkat edilmesi gereken hususlardan birisi de iki trace çağrısı yapmamız. İkinci parçada metodların dönüş yaptığı yerleri izleyeceğimizi belirtmekteyiz.:return normak fonksiyon çağrılarını işaret ederken,:b_return yine tahmin edileceği üzere değer dönüşü olan block'ları belirtmekte.

trace metodları içerisinde işleyişin o anki zamanına ait bir takım bilgileri ekrana bastırıyoruz. Örneğin hangi sınıfın hangi metodunun kullanıldığını, satır numarasını, varsa metod/block dönüş değerlerini ve gerçekleşen olayı bastırmaktayız. Pek tabii bu bilgiler ekran yerine loglama amacıyla farklı veri ortamlarına da kayıt ediliebilirler. Yukarıdaki kod parçasının çalışma zamanı çıktısı ise aşağıdaki gibi olacaktır.

```text
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb #<Class:TracePoint>.trace Line : 7 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Class.inherited Line : 12 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb . Line : 12 Event : class
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Module.attr_accessor Line : 13 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Module.method_added Line : 13 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Module.method_added Line : 13 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Module.method_added Line : 13 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Module.method_added Line : 13 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Module.method_added Line : 15 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Module.method_added Line : 19 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb . Line : 22 Event : end
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Class.new Line : 25 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.initialize Line : 15 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  17 Event : return Value :  ["master vindu", 19000]
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Class.new Line : 26 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.initialize Line : 15 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  17 Event : return Value :  ["luk skay valkr", 21000]
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Class.new Line : 27 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.initialize Line : 15 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  17 Event : return Value :  ["princes leya", 13400]
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Class.new Line : 28 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.initialize Line : 15 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  17 Event : return Value :  ["obi van kinobi", 90980]
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Array.each Line : 29 Event : c_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb . Line : 29 Event : b_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.Move Line : 19 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  21 Event : return Value :  Move to istanbul
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  32 Event : b_return Value :  19010
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb . Line : 29 Event : b_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.Move Line : 19 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  21 Event : return Value :  Move to istanbul
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  32 Event : b_return Value :  21010
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb . Line : 29 Event : b_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.Move Line : 19 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  21 Event : return Value :  Move to istanbul
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  32 Event : b_return Value :  13410
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb . Line : 29 Event : b_call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Player.Move Line : 19 Event : call
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  21 Event : return Value :  Move to istanbul
D:/Users/bsenyurt/rubyWorkSpace/Ruby101/TraceMeSample.rb Line :  32 Event : b_return Value :  90990
```

Dikkat edileceği üzere takip etmek istediğimiz olayların çalışma zamanına ait bilgileri ekrana basılmıştır. Bunu daha düzgün bir formatta yazarsak okunabilir bir Trace çıktısı elde etmemiz içten bile değil. Bu taktiği daha karmaşık Ruby kodlarında uyguladığımızda çalışma zamanında ceyran eden olayları izleyerek işleyiş hakkında detaylı bilgilere sahip olmamız mümkün. Hatta süre bilgisini de ekleyerek metod geçişleri arasındaki farkları izleyebilir ve uygulamanın yavaş işleyen parçalarını tespit edebiliriz. [TracePoint sınıfına ait detaylı bilgilere şu adresten ulaşabilirsiniz.](http://ruby-doc.org/core-2.0.0/TracePoint.html) Böylece geldik bir ruby kod parçacığının daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.
