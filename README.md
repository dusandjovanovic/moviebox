# moviebox
*Universal Windows Application used for Movies and TV Series metadata retrieval. Neo4j graph database integration.*

MovieBox  je aplikacija koji omogućava organizovanje i lako održavanje kolekcije filmova. Održavanje kolekcije podrazumeva dodavanje, modifikaciju i brisanje filmova. Pored filmova postoji i pregled sezona serija. MovieBox omogućava nalaženje odredjenog filma iz kolekcije i reprodukciju sadržaja ukoliko je taj konkretan film prisutan na fajlsistemu korisnika. Pored održavanja biblioteke MovieBox pruža i pribavljanje preporučenih filmova, formiranih na osnovu trenutnog sadržaja biblioteke.


![alt text][main]

[main]: metadata/movies.PNG


### Graph baza Neo4j
Za održavanje perzistencije koristi se neo4j baza podataka u koju se preslikava model podataka prisutan u samoj logici aplikacije.
Neophodno je pokrenuti neo4j runtime iz foldera **neo4j/bin/** komandom `neo4j console`. Ne treba zatvarati konekciju prilikom korišćenja aplikacije.

`username: neo4j`

`password: admin`

Za pristupanje bazi podataka koristi se [neo4jclient](https://github.com/Readify/Neo4jClient).

#### Struktuiranost graph baze

`Nodes: Movie, Genre, Actor, Director, Writer`

`(Movie)-[IS_GENRE]->(Genre)`

`(Movie)<-[ACTS_IN]-(Actor)`

`(Movie)<-[DIRECTED]-(Director)`

`(Movie)<-[WROTE]-(Writer)`


Atributi entiteta **Movie**

`{
  "Path", 
  "Popularity", 
  "VoteAverage" 
  "Title", 
  "Overview", 
  "ReleaseDate", 
  "IsAdultThemed", 
  "BackdropPath", 
  "Runtime", 
  "Year", 
  "VoteCount", 
  "OriginalTitle, 
  "Poster", 
  "Id"
}`

![alt text][movieGraph]

[movieGraph]: metadata/graph.png






`Nodes: TVShow, Genre, TVShowCreator, Season, Network`

`(TVShow)-[IS_GENRE]->(Genre)`

`(TVShow)<-[CREATED]-(TVShowCreator)`

`(TVShow)-[BELONGS]->(Network)`

`(TVShow)-[HAS]->(Season)`


Atributi entiteta **TVShow**:

`{
  "Path", 
  "OriginalLanguage", 
  "Homepage", 
  "NumberOfSeasons", 
  "EpisodeRunTime", 
  "LastAirDate", 
  "InProduction":, 
  "Popularity", 
  "Overview", 
  "FirstAirDate", 
  "OriginalName", 
  "PosterPath", 
  "Name", 
  "BackdropPath", 
  "NumberOfEpisodes", 
  "Id"
}`

![alt text][seriesGraph]

[seriesGraph]: metadata/graph(1).png


#### Pristupanje bazi podataka

Pristupanje bazi podataka ostvaruje se kroz statičku klasu NeoSingleton u kojoj se inicijalizuje instanca konekcije sa bazom.

`if (_instance == null)
                    _instance = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "admin");`

Metode:

`_addMovie(Movie movie)` za dodavanje novog entiteta filma u bazu podataka, kao i dodavanje svih elemenata sa kojima je u vezi.

`_removeMovie(Movie todelete)` za brisanje postojećeg filma i svih veza.

`_modifyMovie(Movie tomodify)` za promenu atributa postojećeg filma.

`_allActors()` za dobijanje liste svih glumaca prisutnih u bazi.

`_allGenres()` za dobijanje liste svih žanrova prisutnih u bazi.

`_allDirectors()` za dobijanje liste svih režisera prisutnih u bazi.

`_allWriters()` za dobijanje liste svih pisaca prisutnih u bazi.

`_readByMultiple(int runtimeLow, int runtimeHigh, int yearLow, int yearHigh, IEnumerable<object> actors, IEnumerable<object> directors, IEnumerable<object> genres)` za dobijanje liste filmova koja odgovara atributima metode (biće objašnjeno kasnije).

`_readByMeta(object actorName, object directorName, object genreName)` za filtriranje filmova po glumcu, režiseru i žanru.

`_readAll()` za dobijanje svih filmova iz baze, kao elemenata koji su u vezi sa njima.


`_readTVShows()` za dovijanje svih serija iz baze, kao elemenata koji su u vezi sa njima.

`_addTVShow(TVShow show)` za dodavanje novog entiteta serije.

`_addSeason(Season newSeason, TVShow show)` za dodavanje sezone seriji koja je već prisutna u bazi.

`_removeTVShow(TVShow todelete)` za brisanje celokupne serije i svih elemenata sa kojima je u vezi.

`_removeSeason(Season todelete)` za brisanje sezone postojeće serije.

#### Cypher query i pristupanje kroz neo4jclient
Primer Cypher naredbi i pristupanja bazi na metodi `ObservableCollection<Movie> _readByMultiple`. Ova metoda se koristi na DetailedView stranici aplikacije. Metoda vrši filtriranje filmova po zadatim atributima, rezultat predstavlja listu filmova u kojoj svaki film u potpinosti odgovara svakom od zadatih atributa. Vreme trajanja treba da bude izmedju donje i gornje granice, godina vezana za film takodje treba da odgovara granicama. Što se tiče glumaca, režisera i žanrova mora da postoji presek izmedju prosledjenih parametara i parametara entiteta filma koji se razmatra. U slučaju poklapanja glumaca dovoljno je da bar jedan od glumaca iz prosledjene liste bude prisutan i razmatranom filmu, isto važi za režisere i žanrove.

Atributi:
* `int runtimeLow` : donja granica trajanja

* `int runtimeHigh` : gornja granica trajanja

* `int yearLow` : donja granica godine koja je veza

* `int yearHigh` : gornja granica godine

* `IEnumerable<object> actors` : lista glumaca

* `IEnumerable<object> directors` : lista režisera

* `IEnumerable<object> genres` : lista žanrova

            var returned = _instance.Cypher
               .Match("(movie:Movie)<-[ACTS_IN]-(actor:Actor)")
               .Match("(movie:Movie)<-[DIRECTED]-(director:Director)")
               .Match("(movie:Movie)-[IS_GENRE]->(genre:Genre)")
               .Where((NeoMovie movie) => movie.Runtime >= runtimeLow)
               .AndWhere((NeoMovie movie) => movie.Runtime <= runtimeHigh)
               .AndWhere((NeoMovie movie) => movie.Year >= yearLow)
               .AndWhere((NeoMovie movie) => movie.Year <= yearHigh)
               .AndWhere("actor.Name in {actorsList}")
               .AndWhere("genre.Name in {genresList}")
               .AndWhere("director.Name in {directorsList}")
               .WithParams(new
               {
                   actorsList,
                   genresList,
                   directorsList
               })
               .ReturnDistinct(movie => movie.As<NeoMovie>())
               .Results;
       

Ovaj konkretan Cypher upit se koristi na **DetailedView** stranici aplikacije. Ovo uključuje prikazivanje svih dostupnih meta podataka o filmu u desnom delu, listu filmova u središnjem i filtriranje liste u levom delu stranice. Filtriranje uključuje izbor ograničenja godine filma, na isti način ograničenje trajanja, izbor žanrova koji treba da odgovaraju svakom filmu u listi, izbor režisera i na kraju izbor glumaca. Svi ovi parametri zajedno odredjuju koji će film biti prikazan u filtriranoj listi. Samo oni filmovi koji odgovaraju svim parametrima filtrirana će biti uvršćeni u rezultujuću listu. Filtriranje se vrši pozivom `ObservableCollection<Movie> _readByMultiple`.

![alt text][details]

[details]: metadata/detailed.PNG




## Opis i slučajevi korišćenja

Početna stranica aplikacije se učitava prilikom svakog pokretanja iste i predsatvlja osnovni pregled svih filmova koji su trenutno prisutni u kolekciji. Sa leve strane se nalazi navigacioni meni, a sa desne strane se nalaze osnovni podaci o trenutno selektovanom filmu. Pri vrhu se redom nalaze mogućnosti dodavanja, modifikacije, brisanja, uvoza filmova iz foldera, sortiranja po alfabetnom redosledu, ukidanje modifikacije prikaza i izbor parametara žanrova, režisera i glumaca.

![alt text][main]

[main]: metadata/movies.PNG

### Navigacioni meni i pregled filmova

Prva stavka navigacionog menija predstavlja pregled svih filmova trenutno prisutnih u kolekciji, inicijalno je izabrana prilikom pokretanja aplikacije. U centralnom delu se nalazi takozvani Grid pregled filmova u kome su svi prisutni filmovi predstavljeni u vidu njihovih poster slika i naziva. Klikom na neki od filmova iz ovog pregleda on je izabran i sa desne strane se mogu videti osnovne informacije o izabranom filmu. Osnovne informacije uključuju veću sliku, naziv, godinu, žanr, režisere i glumce. Pri pokretanju aplikacije inicijalno je izabran pregled svih filmova u kolekciji korisnika. Za stavke sa specificiranim atrbutom putanje se nalazi i informacija o putanji do konkretnog fajla na memorijskom medijumu.

**Osnovno filtriranje prikazanih filmova se zadaje iz drop-down menija Actors, Directors i Genres. Izborom nekog glumca, na primer, biće filtriran prikaz filmova tako da sadrži samo filmove koji su u vezi (u graph bazi) sa zadatim glumcem. Ista analogija važi i za filtriranje po režiserima i žanrovima. Moguće je kombinovanje ovih filtera. U primeru je izabran filter po žanru Adventure.**

Poništavanje svih filtera se vrši klikom na x dugme. Sortiranje trenutne selekcije po alfabetnom redosledu naziva se dobija klikom na sort dugme.

![alt text][filter]

[filter]: metadata/filter.PNG

### Dodavanje filma
Klikom na Add dugme u gornjem meniju otvara se dijalog dodavanja novog filma u kolekciju. Od korisnika se očekuje da unese naziv filma u prvo polje ovog dijaloga, i po mogućstvu putanju do fajla u polju Path. Klikom na dugme Find metadata se pokreće proces pribavljanja meta podataka o filmu. Nakon pribavljanja korisnik dodaje film klikom na dugme Add.

![alt text][add]

[add]: metadata/add.PNG


### Modifikacija filma, ponovno pribavljanje metadapodataka
Klikom na Modify metadata dugme u gornjem meniju otvara se dijalog promene atrbibuta postojećeg filma iz kolekcije. Moguće je promeniti putanju do filma na lokalnom fajlsistemu ili ponovo pribaviti potencijalno promenjene metapodatke filma. Nakon željenih modifikacija korisnik pritiskom na  dugme Modify menja informacije o izabranom filmu. Ova mogućnost je najsmislenija prilkom promene atributa putanje do samog multimedijalog fajla.

### Brisanje filma
Klikom na dugme Remove se briše trenutno selektovani film iz kolekcije, pre samog brisanja se proverava izvršenje sa korisnikom uz Message dijalog. Klikom na Yes, Remove film je nepovratno obrisan iz kolekcije filmova.

### Reprodukcija filmova
Za svaki film koji je prisutan na memorijskom medijumu i pritom je definisan njegov atrbut putanje, prethodno od strane korisnika, prelaskom kursora preko slike filma u osnovnom pregledu javiće se dugme za reprodukciju. Klikom na dugme za reprodukciju prelazi se na stranicu reprodukcije Now Playing..  i momentalno se vrši reprodukcija prethodno selektovanog filma. Korisnik ima mogućnost kontrole reprodukcije, kao i mogućnost Full screen režima.

### Dodavanje više filmova iz foldera
Klikom na browse dugme u gornjem meniju otvara dijalog izbora foldera. Nakon specifikacije foldera pribavljaju se svi multimedijalni fajlovi iz njega i pravi se lista koja će biti prikazana. Za svaki od elemenata liste postoji izbor pribavljanja metapodataka i dodavanja u kolekciju.

![alt text][browse]

[browse]: metadata/browse.PNG


### Pregled sezona serija
Druga stavka navigacionog menija predstavlja pregled svih sezona serija trenutno prisutnih u kolekciji, ovaj pregled se bira klikom na ikonicu druge stavke samog navigacionog menija. U centralnom delu se nalazi takozvani Grid pregled sezona u kome su svi prisutne sve sezone serija predstavljeni u vidu njihovih poster slika i naziva koji uključuje ime serije, odnosno serijala i broj same sezone istog. Klikom na neku od serija iz ovog pregleda ona postaje izabrana i sa desne strane se mogu videti osnovne informacije o izabranoj seriji. Osnovne informacije uključuju veću sliku, naziv, godinu, žanr, pisce, mrežu na kojoj je serija reproukovana i slično. Za stavke sa specificiranim atrbutom putanje se nalazi i informacija o putanji do konkretnog foldera na memorijskom medijumu.

![alt text][series]

[series]: metadata/series.PNG


### Dodavanje sezone
Klikom na Add dugme u gornjem meniju otvara se dijalog dodavanja nove sezone u kolekciju. Od korisnika se očekuje da unese naziv serije i broj sezone, i po mogućstvu putanju do foldera u polju Path. Klikom na dugme Find metadata se pokreće proces pribavljanja meta podataka o sezoni. Nakon pribavljanja korisnik dodaje sezonu klikom na dugme Add.

![alt text][addSeason]

[addSeason]: metadata/addSeason.PNG

### Modifikacija sezone, ponovno pribavljanje metadapodataka
Klikom na Modify dugme u gornjem meniju otvara se dijalog promene atrbibuta postojeće sezone iz kolekcije. Moguće je promeniti putanju do foldera na lokalnom fajlsistemu ili ponovo pribaviti potencijalno promenjene metapodatke sezone. Nakon željenih modifikacija korisnik pritiskom na  dugme Modify menja informacije o izabranoj sezoni.

### Brisanje sezone
Klikom na dugme Remove se briše trenutno selektovana sezona iz kolekcije, pre samog brisanja se proverava izvršenje sa korisnikom uz Message dijalog. Klikom na Yes, Remove sezona je nepovratno obrisana iz kolekcije.

### Preporuka filmova i dodavanje izabranih
Treća stavka navigacinog menija predstavlja pregled preporučenih filmova koji se instantno mogu dodati u kolekciju. Svako pribavljanje je različito i dobija se kao rezultat lista od deset filmova koji su preporučeni korisniku po trenutnom stanju njegove kolekcije. Sam proces pribavljanje se počinje klikom na dugme Refresh u gornjem meniju, nakon pribavljanje kroz listu korisnik prolazi dugmićima levo i desno. Selekcijom Add to collection trenutno izabrani film iz preporučene liste će biti dodat u kolekciju. Nakon selekcije svih željenih filmova potrebno je klikunuti na dugme Add selected i svi selektovani filmovi će biti pridodati kolekciji.

![alt text][recommend]

[recommend]: metadata/explore.PNG

### Detaljan prikaz filmova i filtriranje
Četvrta stavka navigacionog menija predstavlja detaljan pregled filmova. Ovo uključuje prikazivanje svih dostupnih meta podataka o filmu u desnom delu, listu filmova u središnjem i filtriranje liste u levom. Filtriranje uključuje izbor ograničenja godine filma, na isti način ograničenje trajanja, izbor žanrova koji treba da odgovaraju svakom filmu u listi, izbor režisera i na kraju izbor glumaca. Svi ovi parametri zajedno odredjuju koji će film biti prikazan u filtriranoj listi. Samo oni filmovi koji odgovaraju svim parametrima filtrirana će biti uvršćeni u rezultujuću listu.

![alt text][details]

[details]: metadata/detailed.PNG

### Pretraga filmova
Na svakoj stranici aplikacije, u gornjem desnom ugli postoji Search bar za pretraživanje kolekcije. Mogućnost predloga se generiše prilikom kucanja i uvek oslikava trenutno stanje kolekcije. Izborom nekog od filmova se dobija pregled njegovih informacija uz mogućnost instantne reprodukcije istog iz rezultata traženja.

![alt text][search]

[search]: metadata/search.PNG

