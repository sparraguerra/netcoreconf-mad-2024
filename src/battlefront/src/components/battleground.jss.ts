import { createUseStyles } from "react-jss";

export const BattleGroundStyles = createUseStyles({
    container: {
        display: 'flex',
        flexFlow: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        width: '100vw',
        backgroundColor: 'black',
        color: 'white',
        fontFamily: 'Arial',
        backgroundImage: 'url(/images/dragonball-bg.png)',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
    },
    fighters: {
        display: 'flex',
        flexFlow: 'row wrap',
        gap: '20px',
        padding: '20px',
        justifyContent: 'space-around',
        alignItems: 'center',
        width: '100%',
    },
    title: {
        fontSize: '2rem',
        marginBottom: '1rem',
    },
    button: {
        padding: '0.5rem 2rem',
        fontSize: '1rem',
        backgroundColor: 'white',
        color: 'black',
        border: 'none',
        borderRadius: '5px',
        cursor: 'pointer',
    },
    image: {
        width: '100px',
        height: '100px',
    },
    inputContainer: {
        display: 'flex',
        boxSizing: 'border-box',
        flexFlow: 'row',
        justifyContent: 'center',
        alignItems: 'center',
        borderRadius: '12px',
        width: '80%',
    },
    attackInput: {
        padding: '1rem',
        fontSize: '1rem',
        width: '100%',
        border: 'none',
        borderRadius: '5px',
        background: 'none',
        textAlign: 'left',
        backgroundColor: 'rgba(0,0,0,0.7)',
    },
    attackButton: {
        padding: '1rem',
    },
    '@media (max-width: 768px)': {
        container: {
            flexDirection: 'column',
        },
    },
});