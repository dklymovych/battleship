import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper
} from '@mui/material';

const styles = {
  container: {
    maxWidth: 800,
    borderRadius: "20px",
    backgroundColor: "#d9d9d9"
  },
  table_cell: {
    borderColor: "#404040"
  }
}

const rows = [
  { username: "Player1", winRate: 70, numberOfGames: 10 },
  { username: "Player2", winRate: 43, numberOfGames: 20 },
  { username: "Player3", winRate: 10, numberOfGames: 5 }
]

function Scoreboard() {
  return (
    <TableContainer component={Paper} sx={styles.container}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell sx={styles.table_cell}>№</TableCell>
            <TableCell align="right" sx={styles.table_cell}>PLAYER</TableCell>
            <TableCell align="right" sx={styles.table_cell}>WIN RATE</TableCell>
            <TableCell align="right" sx={styles.table_cell}>NUMBER OF GAMES</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {rows.map((row, index) => (
            <TableRow
              key={"Test"}
              sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
            >
              <TableCell component="th" scope="row" sx={styles.table_cell}>{index + 1}</TableCell>
              <TableCell align="right" sx={styles.table_cell}>{row.username}</TableCell>
              <TableCell align="right" sx={styles.table_cell}>{`${row.winRate}%`}</TableCell>
              <TableCell align="right" sx={styles.table_cell}>{row.numberOfGames}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  )
}

export default Scoreboard;
