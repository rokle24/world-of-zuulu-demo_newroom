﻿using System;
using System.Security.Principal;
using Microsoft.VisualBasic;

namespace WorldOfZuul
{
    public class Game
    {
        private Room? currentRoom;
        private Room? previousRoom;
        private Room? village;
        private Room? finalRoom;
        private Room? temp;
        private Inventory_functionality inventory;
        private int roomcount;
        private bool labQuest = true;
        private bool factoryQuest = true;
        private bool westQuest = true;
        private bool eastQuest = true;
        private bool tickTack = true;

        public Game()
        {
            CreateRooms();
            inventory = new Inventory_functionality();
            
        }

        
        private void CreateRooms()
        {
            //upgraded room descriptions&navigation, add strings to rooms you made
            //// you can put all these long strings from this page in separate cs if you want
            //// it make the code ugly
            
            Room? lab = new Room("Laboratory", "\nYou can see village on east",
                "You're in a computing lab full of water samples." +
                "There is only one person who looks like a mad scientist. He starts talking about how over " +
                "2 billion people lack  access  to  safe" +
                " drinking  water and how that leads to over 485.000 people dying along side with " +
                "a million seabirds and marine life every year!" + 
                "He needs a companion to continue his research. Write 'talk' to talk to the Mad Scientist");
            
            Room? village = new("Village", "\nYou see well on east, old building on south and " +
                                           "laboratory on west. There seems to be some passage on north, but " +
                                           "unknown force prevents you from entering",
                "You are standing in the main area of AquaVale village.");
            
            Room? well = new("Well", "\nYou can see sanitation area on east and village on west",
                "You enter the village well. The air is heavy with the smell of stagnant water, and you " +
                "notice algae growing around the edges. A concerned villager named Amara approaches you. She is a " +
                "local water keeper, deeply invested in the well’s condition. Write 'talk' to talk to Amara", 
                "Emerald");
            
            Room? sanitation_area = new("Sanitation Area", "\nYou can see well on west", 
                "The sanitation area is an open plot of land that villagers currently use for waste disposal. "+
                "The ground is littered with refuse, and a foul smell permeates the air. A young carpenter named " +
                "Malik approaches you. He’s eager to help but unsure how to proceed. Write 'talk' to talk to Malik");
            
            Room? factory = new("Factory","\nYou see office door on east and village on north" ,
                "You entered old building, it looks like some kind of ancient factory. Air is very humid, " +
                "you noticed watter dripping from ceiling and mold growing on surrounding objects. Old man in black " +
                "coat is operating some machinery, he looks busy. Write 'talk' to start conversation"  ); 
                  
            
            Room? office = new("Office", "\nYou see some smart device. It introduced itself as Ciri. "+
                                         "It asked you to talk with it, if you want to play a game. Exit to lab " +
                                         "is on east" ,"You entered an office. Everything looks normal, but " +
                                                       "atmosphere here gives you goosebumps, something feels off.", 
                "", "Office key");
            
            Room? finalRoom = new("Water Temple", "\nHere you are able to see your results: " +
                                                       "Did you manage to save the village? Or could you do better? " +
                                                       "Press 'talk' one final time to see how you did", "You have gained access " +
                "to The Water Temple of the village, since you finished all the tasks. Try to explore to see your results." );

            roomcount = 4; //zones that need to be completed before unlocking final room
                           //increase if u add some main quest rooms, make it -- when you complete the room
                           
            this.village = village;
            this.finalRoom = finalRoom;

            village.SetExits(null, well, factory, lab);
            lab.SetExit("east", village);
            well.SetExits(null, sanitation_area, null, village);
            sanitation_area.SetExit("west", well);
            factory.SetExits(village, office, null, null);
            office.SetExit("west", factory);

            currentRoom = village;
        }

        public void Play()
        {
            Parser parser = new();
            PrintWelcome();
            bool continuePlaying = true;

            while (continuePlaying)
            {

                if (roomcount == 0)
                {
                    village.SetExit("north", finalRoom);
                    village.SetDirections("The passage to north is now open. Something is telling you to enter");
                    village.SetDescription("The passage to north is now open. " +
                                           "Something is telling you to enter");
                }
                
                Console.WriteLine("\n"+currentRoom?.ShortDescription);
                Console.Write("> ");

                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("\nPlease enter a command.");
                    continue;
                }

                Command? command = parser.GetCommand(input);

                if (command == null)
                {
                    Console.WriteLine("\nI don't know that command.");
                    continue;
                }

                switch (command.Name)
                {
                    case "explore":
                        Console.WriteLine(currentRoom?.Directions);
                        break;

                    case "map":
                        Map();
                        break;

                    case "back":
                        if (previousRoom == null||currentRoom == finalRoom)
                            Console.WriteLine("\nYou can't go back from here!");
                        else
                        {
                            temp = currentRoom;
                            currentRoom = previousRoom;
                            previousRoom = temp;
                            Console.WriteLine("\n"+currentRoom?.LongDescription);
                        }
                        break;
                    
                   case "talk":
    if (currentRoom?.ShortDescription != null)
    {
        switch (currentRoom.ShortDescription)
        {
            
            //we can add multiple endings based on items you earn from quests
            
            case "Water Temple":
               int score = 0;//12 in total

                if(inventory.HasItem("Water Purifying Chemical"))//lab best
                {
                    score += 2;
                    Console.WriteLine("You cleaned the water by using Water Purifying Chemical for the people of the village!");
                }
                
                if(inventory.HasItem("Water Test Kit"))//lab mid
                {
                    score += 1;
                    Console.WriteLine("You analyze the quality of water by using Water Test Kit but the water is still contaminated."); 
                }

                if(inventory.HasItem("Emerald"))//well best 
                {
                    score += 2;
                    Console.WriteLine("You installed high-quality water filtration system that effectively removes contaminants and impuritiesfrom the water!");
                }

                if(inventory.HasItem("Red Powder"))//well mid
                {
                    score += 1;
                    Console.WriteLine("Educating the villagers how to clean the well is a good idea but not so efficient and it is a temporary solution.");
                }
                 
                if(inventory.HasItem("Diamond"))//office
                {
                    score += 1;
                }

                if(inventory.HasItem("Office key"))//factory
                {
                    score += 2;
                    Console.WriteLine("By repairing the pipelines you ensure that no dirt and bacteria from outside will enter the system.");
                }

                if(inventory.HasItem("Sapphire"))//sanitation area best
                {
                    score += 2;
                    Console.WriteLine("By building durable toilets and washing stations the water usage gets increased significantly!");
                }

                if(inventory.HasItem("Wooden Planks"))//sanitation area mid
                {
                    score += 1;
                    Console.WriteLine("It is not the best idea to build temporary toilets and washing stations since they polute nature\n and are a temporary answer to a long term problem.");
                }

                if(score<= 6)
                {
                    Console.WriteLine("You did not manage to save the village. But don't worry you have learned a lot! Until next time.");
                }
                else if(score > 6 && score < 9)
                {
                    Console.WriteLine("You saved the villgae temporary. But don't worry you have learned a lot! Until next time.");
                }
                else if(score  > 9 && score < 12)
                {
                    Console.WriteLine("CONGRATULATIONS you saved the villge. And still managed to learn something! Until next time.");
                }
                else
                {
                    Console.WriteLine("CONGRATULATIONS you saved the villge! The locals can sleep in pice thanks to you!");
                }
                Environment.Exit(1);
                break;

            case "Village":
                
                Console.WriteLine("\nYou seem to be talking to yourself.");
                break;
            
            case "Office":
                
                if (tickTack)
                {
                    Console.WriteLine("\nCiri: Lets play a game! Can you play Tick Tack Toe? Let's try, " +
                                      "I'm very bad player tho, I's sure you can beat me! \n" +
                                      "Enter coordinates in format column row (example:1 2)\n");
                    if (Ticktack.PlayTickTack())
                    {
                        Console.WriteLine("\nCiri: Wow, you're such a good player! I have a gift for you!");
                        inventory.AddItem(new Item("Diamond", "A shiny Diamond given by Ciri."));
                        tickTack = false;
                        currentRoom.SetDirections("\nYou can see Ciri, the worst Tick Tack Toe player you know.");
                    }
                    else
                    {
                        Console.WriteLine("\nCiri: Wow, I never tough I can win! " +
                                          "Talk with me if you want to try again!");
                    }
                }
                else
                {
                    Console.WriteLine("\nCiri: You're such a good player!");
                }

                
                break;

            case "Laboratory":
                
                if (labQuest)
                {
                    Console.WriteLine(
                "Mad Scientist: The number of deseases in the water is rising rapidly. " +
                "We need to find a solution fast!!!\n" + 
                "1. Develop chemical water treatment methonds by using chemicals to remove impurities and pathogens.\n"+
                "2. Provide testing kits to the community to monitor water contamination.\n" +
                "3. Tell him that you don't help crazy people!");
                    
                    bool wronganswer = true;
                    while (wronganswer)
                    {
                        wronganswer = false;
                        string? labChoice = Console.ReadLine();
                        switch (labChoice)
                        {
                            case "1":
                                Console.WriteLine("You chose the most effective option. The Mad Scientist gives " +
                                                  "you water purifying chemical");
                                inventory.AddItem(new Item("Water Purifying Chemical", "Mix it with " +
                                    "contaminated water and watch the magic happen!"));
                                currentRoom.SetDescription("Lab is looking like new and air is fresh. " +
                                                           "Good job");
                                currentRoom.SetDescription("All water in village is now safe to drink.");
                                break;
                            case "2":
                                Console.WriteLine("You chose a moderately good option. You are given a Water Test " +
                                                  "Kit from the Mad Scientist");
                                inventory.AddItem(new Item("Water Test Kit", "Kit which can be used " +
                                                                             "for testing clarity of water!"));
                                currentRoom.SetDescription(
                                    "The air around is suffocating you. It hurts in your lungs. " +
                                    "You wish to leave this place");
                                currentRoom.SetDescription("Everyone can test his own water, " +
                                                           "people are lazy tho.");
                                break;  
                            case "3":
                                Console.WriteLine(
                                    "\nYour option is ineffective and leads to further sickness. " +
                                    "Scientist is disappointed.");
                                currentRoom.SetDescription("You're bad person!");
                                break;
                            default:
                                Console.WriteLine("\nInvalid choice. Please choose 1, 2, or 3.");
                                wronganswer = true;
                                break;
                        } 
                        
                    }
                    
                    roomcount--;
                    labQuest = false;
                }
                            
                
                else goto default;
                break;
            
            
            case "Well":
                
                if (eastQuest)
                {
                    Console.WriteLine(
                        "\nAmara: Leader, our well is in terrible condition. People are falling sick because of the " +
                        "polluted water. We need a solution, but I can’t decide what’s best. What should we do?\n" +
                        "1. Install a high-quality water filtration system, ensuring clean water for the village.\n" +
                        "2. Clean the well thoroughly and educate villagers on protecting it from contamination.\n" +
                        "3. Ignore the problem for now and hope the water improves naturally."
                    );

                    bool wronganswer = true;
                    while (wronganswer)
                    {
                        wronganswer = false;
                        string? wellChoice = Console.ReadLine();
                        switch (wellChoice)
                        {
                            case "1":
                                Console.WriteLine("\nYou chose the most effective option. " +
                                                  "You receive an emerald from Amara.");
                                inventory.AddItem(new Item("Emerald", "A shiny emerald given by Amara."));
                                currentRoom.SetDescription("Well was replaced by tap with " +
                                                           "modern water filtration system.");
                                break;
                            case "2":
                                Console.WriteLine("\nYou chose a moderately good option, some maintenance will be " +
                                                  "required. You receive red powder from Amara.");
                                inventory.AddItem(new Item("Red Powder", 
                                    "A mysterious red powder given by Amara."));
                                currentRoom.SetDescription("Well is nice and clean");
                                break;
                            case "3":
                                Console.WriteLine("\nYour option is ineffective and leads to further sickness in the " +
                                                  "village. Amara is disappointed.");
                                currentRoom.SetDescription("\"You enter the village well. The air is heavy " +
                                                           "with the smell of stagnant water, and you notice algae " +
                                                           "growing around the edges.");
                                break;
                            default:
                                Console.WriteLine("\nInvalid choice. Please choose 1, 2, or 3.");
                                wronganswer = true;
                                break;
                        }
                    }
                    
                    roomcount--;
                    eastQuest = false;
                }
                
                else goto default;
                break;
            
            
            case "Factory":
                
                if (factoryQuest)
                {
                    Console.WriteLine(
                        "\nOld man: Leader, our water pipes are leaking, damaging my equipment. I've noticed weird "+
                        "health symptoms since this mold started growing everywhere. " +
                        "I need your help, what should I do?\n" +
                        "1. Repair pipes, remove all mold and ensure good air circulation in building.\n" +
                        "2. Tell him to cover his machines with plastic and go to doctor to get some meds.\n" +
                        "3. Tell him he's smart enough to solve it on his own."
                    );
                    
                    bool wronganswer = true;
                    while (wronganswer)
                    {
                        wronganswer = false;
                        string? factoryChoice = Console.ReadLine();
                        switch (factoryChoice)
                        {
                            case "1":
                                Console.WriteLine(
                                    "\nYou chose the most effective option. You receive a key from old man.");
                                inventory.AddItem(new Item("Office key", "A key given by old man."));
                                currentRoom.SetDescription("Factory is looking like new and air is fresh. " +
                                                           "Good job");
                                break;
                            case "2":
                                Console.WriteLine(
                                    "\nYou relieved old man of his symptoms, but he keeps slowly dying thanks to " +
                                    "toxic mold spore exposure.");
                                inventory.AddItem(new Item("Office key", "A key given by old man."));
                                currentRoom.SetDescription(
                                    "The air around is suffocating you. It hurts in your lungs. " +
                                    "You wish to leave this place");
                                break;
                            case "3":
                                Console.WriteLine(
                                    "\nYour option is ineffective and leads to further sickness. " +
                                    "Old man is disappointed.");
                                break;
                            default:
                                Console.WriteLine("\nInvalid choice. Please choose 1, 2, or 3.");
                                wronganswer = true;
                                break;
                        }
                    }
                    roomcount--;
                    factoryQuest = false;
                }
                
                else goto default;
                break;
            

            case "Sanitation Area":

                if (westQuest)
                {
                    Console.WriteLine(
                        "\nMalik: Leader, this area desperately needs proper sanitation facilities. " +
                        "The villagers are falling ill due to poor hygiene. What should we do?\n" +
                        "1. Build durable toilets and washing stations using sustainable materials.\n" +
                        "2. Build temporary latrines while planning permanent facilities.\n" +
                        "3. Do nothing and tell villagers to manage as they are."
                    );
                    
                    bool wronganswer = true;
                    while (wronganswer)
                    {
                        wronganswer = false;
                        string? sanitationChoice = Console.ReadLine();
                        switch (sanitationChoice)
                        {
                            case "1":
                                Console.WriteLine("\nYou chose the most effective option. " +
                                                  "Malik rewards you with a sapphire.");
                                inventory.AddItem(new Item("Sapphire", 
                                    "A radiant sapphire given by Malik."));
                                currentRoom.SetDescription("Modern toilets and showers replaced hole in the " +
                                                           "ground and pond of rotting watter. Good job");
                                break;
                            case "2":
                                Console.WriteLine("\nYou chose a moderately good option. " +
                                                  "Malik provides you with wooden planks for future use.");
                                inventory.AddItem(new Item("Wooden Planks", 
                                    "Sturdy planks provided by Malik."));
                                currentRoom.SetDescription("New latrines smell as bad as hole in the ground " +
                                                           "they used before. But at least it looks better and " +
                                                           "people dont fall inside");
                                break;
                            case "3":
                                Console.WriteLine("\nYour decision disappoints Malik and leaves the sanitation " +
                                                  "area in poor condition.");
                                currentRoom.SetDescription("The ground is littered with refuse, and " +
                                                           "a foul smell permeates the air.");
                                break;
                            default:
                                Console.WriteLine("\nInvalid choice. Please choose 1, 2, or 3.");
                                wronganswer = true;
                                break;
                        }
                    }
                    roomcount--;
                    westQuest = false;
                } 
                
                else goto default;
                break;

            
            default:
                Console.WriteLine("\nThere's no one here to talk to.");
                break;
        }
    }
    else
    {
        Console.WriteLine("\nYou're not in a valid room to talk.");
    }
    break;

                    case "north":
                    case "south":
                    case "east":
                    case "west":
                        Move(command.Name);
                        break;

                    case "quit":
                        continuePlaying = false;
                        break;

                    case "help":
                        PrintHelp();
                        break;

                    case "show":
                        inventory.ShowItems();
                        break;

                    default:
                        Console.WriteLine("I don't know that command.");
                        break;
                }
            }}

        private void Move(string direction)
        {
            if (currentRoom?.Exits.ContainsKey(direction) == true)
            {
                if (currentRoom.Exits[direction].Check(inventory))
                {
                    previousRoom = currentRoom;
                    currentRoom = currentRoom?.Exits[direction];
                    Console.WriteLine("\n"+currentRoom?.LongDescription);
                }
            }
            else
            {
                Console.WriteLine($"\nYou can't go {direction}!");
            }
        }

        private static void PrintWelcome()
        {
            Console.WriteLine("The village of AquaVale has been your home for as long as you can remember.\n" +
                              "Nestled between rolling hills and a winding river, it was once a place of abundance " +
                              "and harmony.\nBut over the years, things have taken a turn for the worse.\nThe river, " +
                              "once a lifeline for the community, now carries pollutants from upstream.\nThe well, " +
                              "a vital source of water, has fallen into neglect, and sanitation facilities are " +
                              "practically nonexistent.\nSickness is rampant, and the villagers are losing hope.");
            Console.WriteLine("\nAs the newly chosen leader of AquaVale, you carry the weight of responsibility.\n" +
                              "The people have placed their trust in you to bring back the village's former glory.\n" +
                              "Your goal is clear: ensure clean water and proper sanitation for everyone.\n" +
                              "But resources are scarce, and time is limited.\nWith only ten turns to make " +
                              "a difference, every decision counts.");
            Console.WriteLine("\nWill you lead AquaVale to a brighter future, or will the village continue to suffer " +
                              "under the weight of its challenges?");
            PrintHelp();
            Console.WriteLine();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("You are lost. You are alone. You wander around the university.");
            Console.WriteLine();
            Console.WriteLine("Navigate by typing 'north', 'south', 'east', or 'west'.");
            Console.WriteLine("Type 'explore' to explore around the area you are in.");
            Console.WriteLine("Type 'back' to go to the previous room.");
            Console.WriteLine("Type 'help' to print this message again.");
            Console.WriteLine("Type 'quit' to exit the game.");
            Console.WriteLine("Type 'show' to display your inventory.");
            Console.WriteLine("Type 'map' to display your map.");
        }

        private static void Map()
        {
            string map = 
            
@"
  __________________________________________________________
 |                   ________                               |
 |                  | Temple |                              |
 |                  |________|                              |
 |                       |                                  |                                               
 |  ____________     _________     ______     ____________  |
 | | Laboratory |___| Village |___| Well |___| Sanitation | |
 | |____________|   |_________|   |______|   |____________| |
 |                       |                                  |
 |                   _________     _______                  |
 |                  | Factory |___|Office |                 |
 |                  |_________|   |_______|                 |
 |__________________________________________________________|";
            Console.WriteLine(map);
        }

       }
    }
    
