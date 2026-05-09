# README - Tutorial Setup Project Unity dari GitHub

## 📌 Deskripsi

Project ini merupakan project Unity yang diambil dari GitHub dan dapat dijalankan menggunakan Unity Editor.

README ini berisi langkah-langkah setup project mulai dari clone repository hingga menjalankan project di Unity.

---

# 🛠️ Persiapan

Sebelum memulai, pastikan sudah menginstall:

* Unity Hub
* Unity sesuai versi project
* Git
* Akun GitHub

---

# 📥 Clone Project dari GitHub

Buka terminal atau CMD lalu jalankan:

```bash
git clone https://github.com/evasbr/sawitzerland.git
```

Masuk ke folder project:

```bash
cd sawitzerland
```

---

# 📂 Membuka Project di Unity Hub

1. Buka Unity Hub
2. Klik **Add Project**
3. Pilih folder project yang sudah di-clone
4. Pilih versi Unity yang sesuai
5. Klik **Open**

---

# ⚠️ Jika Versi Unity Berbeda

Jika muncul warning versi Unity:

* Install versi Unity yang diminta
* Atau gunakan versi yang kompatibel

Versi Unity biasanya dapat dilihat pada file:

```text
ProjectSettings/ProjectVersion.txt
```

Contoh:

```text
m_EditorVersion: 2022.3.10f1
```

---

# 📦 Install Dependencies

Saat project pertama kali dibuka, Unity akan otomatis:

* Import package
* Generate Library folder
* Compile script

Tunggu hingga proses selesai.

Jika ada package yang missing:

1. Buka:

   ```text
   Window → Package Manager
   ```

2. Install package yang dibutuhkan

---

# ▶️ Menjalankan Project

Setelah semua selesai:

1. Buka scene utama di folder:

   ```text
   Assets/Scenes
   ```

2. Klik tombol **Play**

---

# 🧹 Jika Terjadi Error

## Clear Cache Unity

Tutup Unity lalu hapus folder:

```text
Library/
Temp/
Obj/
```

Kemudian buka ulang project.

---

# 🔄 Pull Update dari GitHub

Untuk mengambil update terbaru:

```bash
git pull origin main
```

---

# 🌿 Branching (Opsional)

Membuat branch baru:

```bash
git checkout -b nama-branch
```

Push branch:

```bash
git push origin nama-branch
```

---

# 📤 Commit Perubahan

```bash
git add .
git commit -m "Update project"
git push origin main
```

---

# 📋 Struktur Folder Unity

```text
Assets/          → Asset game
Packages/        → Unity packages
ProjectSettings/ → Konfigurasi project
Library/         → Cache Unity (auto generate)
```

