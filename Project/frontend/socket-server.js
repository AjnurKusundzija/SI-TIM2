import express from "express"
import { createServer } from "http"
import { Server } from "socket.io"
import cors from "cors"

const app = express()
const httpServer = createServer(app)

const io = new Server(httpServer, {
  cors: {
    origin: ["http://localhost:5173", "http://localhost:3000"],
    methods: ["GET", "POST"],
    credentials: true,
  },
})

app.use(cors())
app.use(express.json())

// HTTP endpoint the .NET backend calls to broadcast events
app.post("/emit", (req, res) => {
  const { eventName, payload } = req.body
  if (!eventName) {
    return res.status(400).json({ error: "eventName is required" })
  }
  io.emit(eventName, payload)
  res.json({ ok: true })
})

io.on("connection", (socket) => {
  console.log("Klijent povezan:", socket.id)

  socket.on("disconnect", () => {
    console.log("Klijent odjavljen:", socket.id)
  })
})

const PORT = process.env.PORT || 3001
httpServer.listen(PORT, () => {
  console.log(`Socket.io server pokrenut na portu ${PORT}`)
})
