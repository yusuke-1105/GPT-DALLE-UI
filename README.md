# 🤔 What is this?
This Unity project is a simple tool to use [Azure OpenAI Service API](https://azure.microsoft.com/en-us/products/ai-services/openai-service) and [OpenAI API](https://openai.com) to generate text (GPT) and images (DALLE).

# 💨 What you need to get started
- OpenAI API key  
- Azure OpenAI Service API key  
- Azure OpenAI Service API endpoint url (GPT, DALLE)  
- Unity 2021.3.15 or later  
- Huge Curiosity  

# 🕵️‍♂️ How to set up
1. Open the project in Unity.  
2. Open the scene `GPT and DALLE`.  
3. Select the `EventSystem` game object in the hierarchy.  
4. Enter the API key and endpoint url in the following fields.  
  - Api Key Azure  
  - Url GPT Azure  
  - Url DALLE Azure  
  - Api Key OpenAI  
  to get the API key and endpoint url, please refer to this [link](https://learn.microsoft.com/ja-jp/azure/ai-services/openai/reference).  
5. (optional) modify parameters for GPT and change the font of `Canvas > Response UI > UI GPT > Text` to make your project compatible with your language.  

# 👩‍💻 How to use the UI
Just, type the text you want to generate in the input field and press the submit button. If you select the `GPT` tab, you can generate text. If you select the `DALLE` tab, you can generate an image.  
You can also change the API service (`Azure OpenAI Service` or `OpenAI`) by pressing the `clown` emoji button.  

# 👀 Caution
I(developer) **have not tested Azure OpenAI Service API**. So, there may be some bugs. If you find any bugs, please let me know.

# 📕 Asset packages
- [Free emojis pixel art](https://assetstore.unity.com/packages/2d/gui/icons/free-emojis-pixel-art-231243)  
- [2D Atlas Speech bubbles Alphabet Numbers](https://assetstore.unity.com/packages/2d/environments/2d-atlas-speech-bubbles-alphabet-numbers-88398)  

# 📄 License
This project is licensed under the Apache-2.0 License.