import { Box } from '@mui/material';
import Scoreboard from '../components/Scoreboard/Scoreboard';
import ScoreboardTitle from '../components/Scoreboard/ScoreboardTitle';
import Sidebar from '../components/Sidebar/Sidebar';

function Main() {
  return (
    <>
      <Sidebar />
      <Box sx={{
        margin: "0 0 0 15vw",
        display: "flex",
        justifyContent: "center"
      }}>
        <Box sx={{
          marginTop: "30px"
        }}>
          <ScoreboardTitle />
          <Scoreboard />
        </Box>
      </Box>
    </>
  )
}

export default Main;
