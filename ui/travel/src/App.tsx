import './App.css'
import RootLayout from './components/Layout/RootLayout';
import signalRService from './services/streamingService'

signalRService.initialize();

function App() {

  return (
    <>
      <RootLayout />
    </>
  )
}

export default App
