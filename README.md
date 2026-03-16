# 🥘 **CookBase** - Aplikacja do zarządzania przepisami kulinarnymi

CookBase to aplikacja mobilna stworzona w technologii **.NET MAUI**, służąca do lokalnego zarządzania przepisami kulinarnymi i produktami spożywczymi. Aplikacja działa w pełni offline, przechowując dane w bazie **SQLite**.

---

## 🚀 **Funkcjonalności**

- **Zarządzanie produktami spożywczymi**:
  - 🥛 Dodawanie produktów z przypisaną kategorią (np. nabiał, mięso, warzywa).
  
- **Tworzenie przepisów**:
  - 🍴 Umożliwia tworzenie przepisów kulinarnych z możliwością wyboru składników z listy lub dodawania nowych.
  - 📝 Przepis zawiera:
    - Nazwę.
    - Kroki przygotowania.
    - Czas przygotowania.
    - Poziom trudności.
    - Zdjęcie gotowej potrawy.

---

## 🧩 **Architektura**

Aplikacja została zaprojektowana zgodnie z architekturą **MVVM** (Model-View-ViewModel), co zapewnia łatwą skalowalność i testowalność aplikacji. 

- **Model**: Zawiera dane aplikacji, takie jak przepisy i produkty.
- **View**: Interfejs użytkownika, wyświetlający dane w odpowiednich ekranach.
- **ViewModel**: Logika biznesowa, która łączy Model z View, zarządza danymi i obsługuje logikę aplikacji.

---

## 🛠 **Technologie**

- **.NET MAUI** - framework do tworzenia aplikacji mobilnych działających na wielu platformach.
- **SQLite** - lokalna baza danych do przechowywania danych offline.
- **MVVM** - architektura aplikacji.

---

## 📱 **Użycie**

Po uruchomieniu aplikacji użytkownik może:

- Dodawać produkty do bazy danych.
- Tworzyć nowe przepisy kulinarne.
- Przeglądać pozycje

---

## 🎨 **Przykłady**

### 1. Dodawanie produktu
Aby dodać produkt, użytkownik wybiera kategorię, wprowadza nazwę produktu i zapisuje go w bazie danych. Można dodać dowolną ilość produktów.

### 2. Tworzenie przepisu
Aby stworzyć przepis, użytkownik wybiera składniki z listy lub dodaje nowe, określa kroki przygotowania, czas, poziom trudności oraz oznaczenie wege/wegan.

### 3. Edycja produktu
Możliwość edycji dodanego produktu.

### 4. Przeglądanie kategorii, przepisów
Możliwość wygodnego i przejrzystego przeglądania dodanych pozycji z kategorii oraz przepisów.

### 5. Przeglądanie oraz sortowanie produktów
Możliwość przeglądania oraz sortowania produktow na podstawie nazwy oraz wg. kategorii.
