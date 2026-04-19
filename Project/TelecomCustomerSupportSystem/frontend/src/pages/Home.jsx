import { useEffect } from "react"

export default function Home() {

  useEffect(() => {
    const ws = new WebSocket("ws://localhost:5000/ws")

    ws.onopen = () => console.log("WS povezan")
    ws.onmessage = (msg) => console.log("WS poruka:", msg.data)
  }, [])

  return <h1>Frontend radi ✅</h1>
}