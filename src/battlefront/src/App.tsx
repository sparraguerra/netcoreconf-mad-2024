import { Battleground } from './components/battleground'
import TenkaichiBudokaiServiceService from './services/api/tenkaichiBudokaiService';

function App() {
  // const [count, setCount] = useState(0)

  const tenkaichiBudokaiService = new TenkaichiBudokaiServiceService();
  tenkaichiBudokaiService.initBattle();

  return (
    <>
      <Battleground />
    </>
  )
}

export default App
