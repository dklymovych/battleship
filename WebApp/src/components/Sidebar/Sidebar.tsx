import { Button } from "@mui/material";
import styles from "./Sidebar.module.css"
import UserIcon from '@mui/icons-material/Person';

const buttonStyles = {
  color: `#404040`,
  border: `1px solid #404040`,
  borderRadius : `15px`,
  paddingLeft: `40px`,
  paddingRight: `40px`,
  height: `40px`,
  width: `200px`,
  ":hover": {
    backgroundColor : `rgba(30,30,30,0.12)`,
    borderColor: "black",
  }
};

function Sidebar() {
  return( 
  <div className={styles.sidebarContainer}>
    <div className={styles.sidebarNavbar}>
      <div className={`${styles.logoContainer} notable-regular`}>
        BATTLESHIP
      </div>
      <div className={styles.userComponent}>
        <div className={styles.userBox}>
        <div className={styles.userIconContainer}>
          <UserIcon/>
        </div>
        <span className={`${styles.rubikTextColor} rubik-regular`}>Username</span>
        </div>
      </div>
      <div className={styles.buttonHolder}>
        <Button variant="outlined" 
        sx={buttonStyles}>
        <span className={`${styles.rubikTextColor} rubik-regular`}>Scoreboard</span></Button>
        <Button variant="outlined" 
        sx={buttonStyles}>
        <span className={`${styles.rubikTextColor} rubik-regular`}>Create Game</span></Button>
        <Button variant="outlined" 
        sx={buttonStyles}>
        <span className={`${styles.rubikTextColor} rubik-regular`}>Join Game</span></Button>
        <Button variant="outlined" 
        sx={buttonStyles}>
        <span className={`${styles.rubikTextColor} rubik-regular`}>Scoreboard</span></Button>
      </div>
    </div>
  </div>
  );
}
export default Sidebar;