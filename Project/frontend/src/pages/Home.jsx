import { useEffect } from "react"
import { io } from "socket.io-client"
import { useNavigate } from "react-router-dom"
import { useAuth } from "../context/AuthContext"
import { Headphones, LayoutDashboard, LogIn, ChevronRight } from "lucide-react"

export default function Home() {
  const { user } = useAuth()
  const navigate = useNavigate()

  useEffect(() => {
    const socket = io("http://localhost:3001")

    socket.on("connect", () => console.log("Socket.io povezan:", socket.id))
    socket.on("notification", (data) => console.log("Notifikacija:", data))

    return () => socket.disconnect()
  }, [])

  return (
    <div className="min-h-screen bg-gradient-to-br from-navy-900 via-navy-800 to-navy-950 flex flex-col">
      {/* Top Bar */}
      <header className="w-full px-6 py-4 flex items-center justify-between bg-navy-900/50 backdrop-blur-sm border-b border-navy-700 sticky top-0 z-10">
        <div className="flex items-center gap-3">
          <div className="flex items-center justify-center w-10 h-10 rounded-xl bg-navy-700 shadow-md">
            <Headphones size={20} className="text-white" />
          </div>
          <span className="text-xl font-bold text-white tracking-wide">
            TelecomSupport
          </span>
        </div>

        <div>
          {user ? (
            <button
              onClick={() => navigate('/dashboard')}
              className="flex items-center gap-2 px-4 py-2 bg-navy-700 hover:bg-navy-600 text-white rounded-lg transition-colors font-medium text-sm shadow-md"
            >
              <LayoutDashboard size={18} />
              Dashboard
            </button>
          ) : (
            <button
              onClick={() => navigate('/login')}
              className="flex items-center gap-2 px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white rounded-lg transition-colors font-medium text-sm shadow-md"
            >
              <LogIn size={18} />
              Prijava
            </button>
          )}
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 flex flex-col items-center p-6 mt-10 md:mt-20">
        {/* Hero Section */}
        <div className="max-w-4xl w-full text-center space-y-6 mb-20 animate-fade-in-up">
          <h1 className="text-4xl md:text-6xl font-extrabold text-white leading-tight">
            Besprijekorna podrška za <br className="hidden md:block" />
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-emerald-400 to-cyan-400">
              telekomunikacione usluge
            </span>
          </h1>
          <p className="text-lg md:text-xl text-navy-200 max-w-2xl mx-auto leading-relaxed">
            Moderan sistem za podršku dizajniran za brže rješavanje problema i pružanje izuzetne korisničke usluge.
            Prijavite se da biste podnijeli zahtjeve, pratili napredak i direktno komunicirali sa našim timom za podršku.
          </p>
          <div className="pt-6">
             <button
               onClick={() => navigate(user ? '/dashboard' : '/login')}
               className="inline-flex items-center gap-2 px-6 py-3 bg-white text-navy-900 hover:bg-gray-100 font-semibold rounded-xl transition-transform hover:scale-105 shadow-xl"
             >
               Započnite odmah
               <ChevronRight size={20} />
             </button>
          </div>
        </div>

      </main>

      {/* Footer */}
      <footer className="py-6 text-center border-t border-navy-800/50 mt-auto bg-navy-950/30">
        <p className="text-navy-400 text-sm">© {new Date().getFullYear()} TelecomSupport. Sva prava zadržana.</p>
      </footer>
    </div>
  )
}
