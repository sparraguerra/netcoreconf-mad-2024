import { createUseStyles } from "react-jss";

export const BattleLogsStyles = createUseStyles({
    container: {
        flex: '0 1 400px',
        display: 'flex',
        maxHeight: '80vh',
        flexFlow: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        padding: '20px',
        backgroundColor: 'rgba(0,0,0,0.7)', // Dark blue color
        color: 'white',
        borderRadius: '12px',
        overflowY: 'scroll',
        gap: '10px',
    },
    message: {
        backgroundColor: 'rgba(30,144,255,0.5)', // Dodger blue color
        padding: '10px',
        borderRadius: '5px',
        fontSize: '1.5rem',
    },
})
