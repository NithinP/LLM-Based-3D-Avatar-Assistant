import os
import sys
from langchain import memory
from langchain.prompts import ChatPromptTemplate, PromptTemplate
from langchain_core.prompts import MessagesPlaceholder
from langchain.chains import LLMChain, LLMMathChain
from langchain.memory import ConversationBufferMemory
from langchain.agents import create_react_agent, AgentExecutor,Tool,tool
from langchain_community.tools import DuckDuckGoSearchRun, WikipediaQueryRun
from langchain_community.utilities import WikipediaAPIWrapper
from langchain_community.utilities.wolfram_alpha import WolframAlphaAPIWrapper
import importlib.util
from pathlib import Path
from datetime import datetime
from langchain_core.runnables.history import RunnableWithMessageHistory
import UnityEngine


base_data_path = Path("D:/3DavatarAssistant/Backend/Client/Client Side/Assets/Scripts/PythonSkriptz/BASE_DATA.py")

sys.path.insert(0, str(base_data_path.parent))

spec = importlib.util.spec_from_file_location("BASE_DATA", base_data_path)
g_c = importlib.util.module_from_spec(spec)
spec.loader.exec_module(g_c)

current_date = datetime.now().strftime("%Y-%m-%d")
current_time = datetime.now().strftime("%H:%M:%S")

g_c.history = g_c.load_memory_from_json('conversation_log.json')


os.environ["WOLFRAM_ALPHA_APPID"] = "WPLQE3-XRWLLJ2JL4"

search = DuckDuckGoSearchRun()
wikipedia = WikipediaQueryRun(api_wrapper=WikipediaAPIWrapper())
wolfram = WolframAlphaAPIWrapper()

word_problem_template = """You are a reasoning agent tasked with solving 
the user's logic-based questions. Logically arrive at the solution, and be 
factual. In your answers, clearly detail the steps involved and give the 
final answer. Provide the response in bullet points.
history: {chat_history}
Question: {question}
Answer:"""

math_assistant_prompt = PromptTemplate(
    input_variables=["question","chat_history"],
    template=word_problem_template
)

word_problem_chain = math_assistant_prompt | g_c.llm
problem_chain = LLMMathChain.from_llm(llm=g_c.llm)

word_message_history = RunnableWithMessageHistory(
    word_problem_chain,
    lambda session_id: g_c.history,
    input_messages_key="input",
    history_messages_key="chat_history",
)

@tool
def Basic_LLM_Response(topic: str) -> str:
    '''
    Generate a basic LLM response for a given topic.
    '''
    template = """You are an agent tasked with Giving response to user general questions
        history: {chat_history}
        Question: {input}
        Answer:"""
    prompt = PromptTemplate(
    input_variables=["input","chat_history"],
    template=template
    )
    chain = prompt | g_c.llm
    Base_message_history = RunnableWithMessageHistory(
    chain,
    lambda session_id: g_c.history,
    input_messages_key="input",
    history_messages_key="chat_history",
)

    return Base_message_history.invoke(
        {"input": topic}, {"configurable": {"session_id": "unused"}}
    )
@tool
def LLM_reasoning(question: str) -> str:
    """
    A tool for solving logic-based questions using an LLM.
    """
    return word_message_history.invoke(
        {"input": question}, {"configurable": {"session_id": "unused"}}
    )
@tool
def calculator_tool(question: str) -> str:
    """
    A tool for solving basic math using an LLM.
    """
    return problem_chain.run(question)
@tool
def advanced_math_tool(question: str) -> str:
    """
    A tool for solving advanced math using LLM.
    """
    return wolfram.run(question)

tools = [
    Tool(
        name="DuckDuckGo Search",
        func=search.run,
        description="Useful to get info from the internet. Use by default."
    ),
    Tool(
        name="Basic LLM Response",
        func=Basic_LLM_Response,
        description="Use this tool to answer things such as greetings, casual chat, and general questions."
    ),
    Tool(
        name="Wikipedia Search",
        func=wikipedia.run,
        description='''A useful tool for searching the Internet 
        to find information on world events, issues, dates, years, etc. Use precise questions.'''
    ),
    Tool(
        name="Reasoning Tool",
        func=LLM_reasoning,
        description="Useful for when you need to answer logic-based/reasoning questions."
    ),
    Tool(
        name="Calculator",
        func=calculator_tool,
        description="Useful for when you need to answer numeric questions. This tool is only for math questions and nothing else. Only input math expressions, without text.",
    ),
    Tool(
        name="Advanced Math",
        func=advanced_math_tool,
        description="Useful for when you need to answer advanced math questions. This tool is only for advanced math questions and nothing else."
    )
]

template = '''Answer the following questions as best you can. You have access to the following tools:
{tools}
Use the following format:
Question: the input question you must answer
Thought: you should always think about what to do
Action: the action to take, should be one of [{tool_names}]
Action Input: the input to the action
Observation: the result of the action
(this Thought/Action/Action Input/Observation can repeat N times)
Thought: I now know the final answer
Final Answer: the final answer to the original input question
Begin!
Question: {input}
history: {chat_history}
Thought:{agent_scratchpad}'''

prompt = PromptTemplate.from_template(template)
agent = create_react_agent(g_c.llm, tools, prompt)

agent_executor = AgentExecutor(
    agent=agent,
    tools=tools,
    verbose=True,
    return_intermediate_steps=True,
)

agent_with_message_history = RunnableWithMessageHistory(
    agent_executor,
    lambda session_id: g_c.history,
    input_messages_key="input",
    history_messages_key="chat_history",
)

def process_input(input_text):
    try:
        response = agent_with_message_history.invoke(
            {"input": input_text},
            {"configurable": {"session_id": "unused"}},
        )
        UnityEngine.Debug.Log("Output:", response['output'])
        g_c.log_conversation(input_text, response['output'])
        sys.stdout.flush()
    except Exception as e:
        print(f"An error occurred: {str(e)}")
        sys.stdout.flush()

if __name__ == "__main__":
    process_input("Hello How Are You")