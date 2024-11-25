import { FighterStyles } from "./fighter.jss";

interface IFighterProps {
    name: string;
    endpoint: string;
    displayGenki: boolean;
    health: number;
    handleAttack: () => void;
    handleGenki?: () => void;
}

export const Fighter = (props: IFighterProps) => {
    const styles = FighterStyles();

    return (
        <div className={styles.container}>
            <img className={styles.image} src={`/images/${props.name.toLowerCase()}.png`} alt={props.name} />
            <div>
                <h1 className={styles.title}>{props.name}</h1>
                <h2 className={styles.title}>{props.health}</h2>
                <button className={styles.attackButton}
                    onClick={() => props.handleAttack()}>
                    Attack
                </button>
                {props.displayGenki && (
                    <>
                        <br />
                        <button className={styles.attackButton}
                            onClick={() => props.handleGenki()}>
                            Genkidama
                        </button>
                    </>
                )}
            </div>
        </div>
    )
}