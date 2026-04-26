import { useEffect } from "react"
import { io } from "socket.io-client"

export default function Home() {

  useEffect(() => {
    const socket = io("http://localhost:3001")

    socket.on("connect", () => console.log("Socket.io povezan:", socket.id))
    socket.on("notification", (data) => console.log("Notifikacija:", data))

    return () => socket.disconnect()
  }, [])

  return <h1>Frontend radi ✅</h1>
}