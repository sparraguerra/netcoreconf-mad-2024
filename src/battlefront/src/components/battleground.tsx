import { useEffect, useState } from "react";
import { BattleGroundStyles } from "./battleground.jss";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { BattleLogsStyles } from "./battlelogs.jss";
import { Fighter } from "./fighter";
import { SystemPrompt } from "../services/aoai/prompt";
import { getClient } from "../services/aoai/chat";
import GokuService from "../services/api/gokuService";
import FreezerService from "../services/api/freezerService";
import ApiService from "../services/api/apiService";
import { CombatHandler } from "../services/aoai/functions";
import Markdown from "react-markdown";
// import { SignalRService } from "../services/signalrService";

export interface ISignalRNegotiation {
    Url: string;
    AccessToken: string;
}

interface IConversation {
    messages: { role: string, content: string }[];
    model: string;
    tools: object[];
    tool_choice: string;
}

interface IAttack {
    power: number;
    missed: boolean;
    is_genki_dama: boolean;
}

export const Battleground = () => {
    const styles = BattleGroundStyles();
    const logStyles = BattleLogsStyles();
    const [negotiation, setNegotiation] = useState<ISignalRNegotiation | null>(null);
    // const [connection, setConnection] = useState<HubConnection | null>(null);
    // const [message, setMessage] = useState("");
    const [url, setUrl] = useState("");
    const [accessToken, setAccessToken] = useState("");
    const [messages, setMessages] = useState<{ role: string, content: string }[]>([
        {
            role: "system",
            content: SystemPrompt
        }
    ]);
    const [attackInput, setAttackInput] = useState('');
    const client = getClient();
    const [gokuHealth, setGokuHealth] = useState(1000);
    const [freezerHealth, setFreezerHealth] = useState(1000);

    // const signalRResponse = signalRService.getSignalRConnectionInfo();

    const addMessage = (role: string, content: string) => {
        return { role, content };
    };

    const gokuService = new GokuService();
    const freezerService = new FreezerService();

    const handleGokuAttack = () => {
        sendAttack("Goku ataca a freezer");
    }
    const handleGenkiDama = () => {
        sendAttack("Goku lanza Genki Dama a freezer");
    }
    const handleFreezerAttack = () => {
        sendAttack("Freezer ataca a goku");
    }

    const sendAttack = async (buttonMessage?: string) => {
        let attackInstruction = buttonMessage || attackInput;
        const userMessage = attackInstruction;
        const historic = [...messages, addMessage('user', userMessage)];
        setMessages(historic);
        console.log(messages);
        setAttackInput('');

        const conversation: IConversation = {
            messages: historic,
            model: "gpt-4o", // Reemplaza con el modelo que estés usando
            tools: CombatHandler,
            tool_choice: "required",
        };

        try {
            const response = await client.chat.completions.create(conversation);
            const result_args = response.choices[0]?.message?.tool_calls[0]?.function?.arguments;
            console.log("Result" + result_args);
            const result = JSON.parse(result_args);
            console.log("Json Result" + JSON.parse(result_args));
            const resultMessage = result.attackinfo;
            console.log("ResultMessage: " + resultMessage);
            await handleAttack(result.attacker, result.attacks, result.is_genki_dama);
            setMessages([...historic, addMessage('assistant', resultMessage + `\n ${await getHealthPoints()}`)]);
        } catch (error) {
            console.error('Error sending message:', error);
        }
    };

    const getHealthPoints = async () => {
        try {
            await new Promise(resolve => setTimeout(resolve, 2000));
            const gokuHealth = await gokuService.getHealthPoints();
            const freezerHealth = await freezerService.getHealthPoints();
            return '\n* Vida restante de Goku: ' + gokuHealth + ' \nVida restante de Freezer: ' + freezerHealth;
        } catch (error) {
            console.error('Error getting health points:', error);
            return 'Error retrieving health points';
        }
    }

    const handleAttack = async (attacker: string, attacks: IAttack[], isGenkiDama: boolean) => {
        let battleService: ApiService;
        if (attacker === 'Goku') {
            battleService = gokuService;
        } else {
            battleService = freezerService;
        }

        attacks.forEach(async attack => {
            if (attack.missed) {
                console.log(`${attacker} missed the attack`);
                return;
            } else {
                if (isGenkiDama) {
                    try {
                        const response = await (battleService as GokuService).launchGenkiDama(attack.power);
                        console.log(`${attacker} attack response:`, response);
                    } catch (error) {
                        console.error('Error calling Battle service:', error);
                    }
                } else {
                    try {
                        const response = await battleService.attack(attack.power);
                        console.log(`${attacker} attack response:`, response);
                    } catch (error) {
                        console.error('Error calling Battle service:', error);
                    }
                }
            }
        })
    };

    useEffect(() => {
        // const newConnection = new HubConnectionBuilder()
        //     // .withUrl("https://sigr-acadragonball.service.signalr.net/client/?hub=tenkaichibudokai")
        //     .withUrl("https://tenkaichibudokai.azurewebsites.net/api")
        //     .withAutomaticReconnect()
        //     .build();
        //     console.log(newConnection.baseUrl);
        // setConnection(newConnection);

        fetch("https://tenkaichibudokai.azurewebsites.net/api/negotiate", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
        })
            .then(response => response.json())
            .then(data => {
                setNegotiation(data);
                setUrl(data.Url);
                setAccessToken(data.AccessToken);
                // console.log(data)
            })
            .catch((error) => console.error('Error:', error));

    }, []);

    useEffect(() => {
        if (url) {
            console.log(2);
            const options = {
                accessTokenFactory: () => accessToken,
            };
            const connection = new HubConnectionBuilder()
                .withUrl(url, options)
                .withAutomaticReconnect()
                .build();

            connection.start()
                .then((result) => {
                    console.log('Connected!');
                    connection.on('tenkaichibudokai-freezer', message => {
                        console.log('Received message: ', message);
                        setMessage(message);
                    });
                    connection.on('tenkaichibudokai-goku', message => {
                        console.log('Received message: ', message);
                        setMessage(message);
                    });
                    console.log("RESULT " + result);
                })
                .catch(e => console.log('Connection failed: ', e));
        }
    }, [negotiation]);

    useEffect(() => {
        const fetchHealthPoints = async () => {
            setGokuHealth(await gokuService.getHealthPoints());
            setFreezerHealth(await freezerService.getHealthPoints());
        };
        fetchHealthPoints();
    }, [messages]);

    return (
        <>
            <div className={styles.container}>
                <div className={styles.fighters}>
                    <Fighter
                        name="Goku"
                        health={gokuHealth}
                        handleAttack={handleGokuAttack}
                        handleGenki={handleGenkiDama}
                        endpoint="https://ca-songoku--4ua9qgj.purpleocean-c5769fe7.westeurope.azurecontainerapps.io/attack-freezer" displayGenki={true} />
                    <Fighter
                        name="Freezer"
                        health={freezerHealth}
                        handleAttack={handleFreezerAttack}
                        endpoint="https://ca-freezer.purpleocean-c5769fe7.westeurope.azurecontainerapps.io/attack-goku" displayGenki={false} />
                    <div className={logStyles.container}>
                        {messages.map((msg, index) => (
                            msg.role === 'assistant' ? (
                                <div className={logStyles.message} key={index}>
                                    <Markdown>{msg.content}</Markdown>
                                </div>
                            ) : <></>
                            // <div className={logStyles.message} key={index}>
                            //     {msg.role === 'assistant' ? (<Markdown>{msg.content}</Markdown>) : null}
                            // </div>
                        ))}
                    </div>
                </div>
                <div className={styles.inputContainer}>
                    <input
                        type="text"
                        className={styles.attackInput}
                        value={attackInput}
                        onChange={(e) => setAttackInput(e.target.value)}
                        placeholder="Type attack..."
                        onKeyDown={(e) => { if (e.key === 'Enter') { sendAttack(); } }}
                    />
                    <button className={styles.attackButton} onClick={sendAttack}>Send</button>
                </div>
            </div>
        </>
    )
}