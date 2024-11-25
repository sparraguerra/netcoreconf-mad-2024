export const CombatHandler = [
    {
        "type": "function",
        "function": {
            "name": "combat_handler",
            "description": "This function handles the combat between Goku and Freezer",
            "parameters": {
                "type": "object",
                "properties": {
                    "attackinfo": {
                        "type": "string",
                        "description": `Verbose description of the attack with the following information in different lines separated by an markdown line break:
                            * attacker
                            * power of the attack
                            * if it was missed`,
                    },
                    "attacker": { 
                        "type": "string",
                        "description": "The attacker's name, e.g. Goku or Freezer",
                    },
                    "attacks": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "power": {
                                    "type": "number",
                                    "description": "The power of the attack",
                                },
                                "missed": {
                                    "type": "boolean",
                                    "description": "Indicates if the attack was dodged",
                                },
                                "is_genki_dama": {
                                    "type": "boolean",
                                    "description": "Indicates if the attack is a Genki Dama",
                                },
                            },
                            "required": ["power", "missed"],
                        },
                    },
                    "goku_health": {
                        "type": "number",
                        "description": "The remaining health of Goku",
                    },
                    "freezer_health": {
                        "type": "number",
                        "description": "The remaining health of Freezer",
                    },
                },
                "required": ["attackinfo", "attacker", "attacks", "goku_health", "freezer_health"],
            },
        }
    }
]