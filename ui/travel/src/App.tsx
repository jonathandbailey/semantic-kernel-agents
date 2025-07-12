import './App.css'
import ConversationPage from './pages/ConversationPage'
import signalRService from './services/streamingService'

signalRService.initialize();

function App() {

  return (
    <>
      <ConversationPage />
    </>
  )
}

export default App
