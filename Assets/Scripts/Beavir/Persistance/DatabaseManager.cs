using RenderHeads.UnityOmeka.Data;
using RenderHeads.UnityOmeka.Data.Vocabularies;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using RenderHeads.UnityOmeka.Core.Extensions;
using RenderHeads.UnityOmeka.Core;
using RenderHeads.UnityOmeka.Core.Interface;
using RenderHeads.UnityOmeka.Core.Impl;

namespace Beavir.Persistance.Database
{
    /// <summary>
    /// Controller that manages the data from the OmekaS database (both text data and images)
    /// </summary>
    public class DatabaseManager : NetworkBehaviour
    {
        [SerializeField] private ClientSettings _clientSettings = null;
        [SerializeField] private GameObject[] dBReportsCollection;
        [SerializeField] private GameObject[] icpPMReportsCollection;
        [SerializeField] private GameObject[] icpReportsCollection;
        [SerializeField] private GameObject[] plmReportsCollection;
        [SerializeField] private GameObject[] semReportsCollection;
        [SerializeField] private GameObject[] drReportsCollection;
        [SerializeField] private GameObject[] omForSemReportsCollection;
        [SerializeField] private bool printDB;
        [SerializeField] public float delay;
        private string[] gpText = new string[3]; //id, title, description
        private string[] icpPMText = new string[3]; //id, title, description
        private string[] icpText = new string[3]; //id, title, interpdetails
        private string[] drText = new string[3]; //id, description, interpdetails
        private string[] semText = new string[3]; //id, title, description
        private string[] omForSemText = new string[3]; //id, title, description
        private string[] plmText = new string[2]; //id, description
        private byte[] icpPMImage;
        private byte[] drImage;
        private byte[] semImage;
        private byte[] omForSemImage;
        private int processedImages = 0;
        private float timer;
        private bool called = false;

        public int ProcessedImages { get => processedImages; }
        /// <summary>
        /// Reference to our api business logic. The type definition defines the vocabulary that api will know about.
        /// </summary>
        public IAPI<DublicCoreVocabulary> Api { get; private set; }

        void Start()
        {
            if (_clientSettings == null)
            {
                throw new System.Exception("[OmekaClient] Init failed, client settings is null");
            }
            if (Api == null)
            {
                IAPI<DublicCoreVocabulary> api = new StandardApi<DublicCoreVocabulary>();
                Api = api;
                api.SetRestEndPoint(_clientSettings.OmekaEndpoint);
                api.SetCredentials(_clientSettings.KeyIdentity, _clientSettings.KeyCredential);
                //Debug.Log("[OmekaClient] Successfully Initialized");
            }
        }

        [ServerCallback]
        void Update()
        {
            if (!called)
            {
                timer += Time.deltaTime;
                if (timer > delay)
                {
                    called = true;
                    Initialize();
                }
            }
        }


        public async void Initialize()
        {
            try
            {
                ItemSetSearchResponse<DublicCoreVocabulary> response = await SearchItemSets();
                IdObject idObject = response.ItemSets[0].Id;
                ItemSearchResponse<DublicCoreVocabulary> items = await SearchItemSet(idObject);

                UpdateGPCanvases(items);
                UpdateIcpPMCanvases(items);
                UpdateIcpCanvases(items);
                UpdateSEMCanvases(items);
                UpdateDRCanvases(items);
                UpdatePLMCanvases(items);
                UpdateOMforSEMCanvases(items);
            }
            catch (System.Exception)
            {
                processedImages = -1;
                print("Problems connecting with the database");
            }
        }

        /// <summary>
        /// Callback that searchs for item sets
        /// </summary>
        public async Task<ItemSetSearchResponse<DublicCoreVocabulary>> SearchItemSets()
        {
            ItemSetSearchResponse<DublicCoreVocabulary> response = await Api.SearchItemSets(new ItemSetsSearchParams()
            {
                id = 6506
            });

            return response;
        }


        /// <summary>
        /// Search a specific item set and returns all its items
        /// </summary>
        /// <param name="index">The ID of the item set to search</param>
        public async Task<ItemSearchResponse<DublicCoreVocabulary>> SearchItemSet(IdObject index)
        {
            ItemSearchResponse<DublicCoreVocabulary> response = await Api.SearchItems(new ItemSearchParams()
            {
                item_set_id = index.Id,
                per_page = 300,
            });

            return response;
        }

        private void UpdateGPCanvases(ItemSearchResponse<DublicCoreVocabulary> response)
        {
            foreach (var report in dBReportsCollection)
            {
                foreach (var item in response.Items)
                {
                    if (int.Parse(report.name) == item.Id.Id)
                    {
                        gpText[0] = (item.Id.Id).ToString();
                        gpText[1] = item.Title;
                        gpText[2] = item.Vocabulary.DCTermsDescription[0].Value + "\n \n" + item.Vocabulary.DCTermsDescription[1].Value;
                        UpdateSingleGPCanvas(gpText);
                    }

                }
            }
        }

        [ClientRpc]
        private void UpdateSingleGPCanvas(string[] text)
        {
            foreach (var report in dBReportsCollection)
            {
                if (report.name == text[0])
                {
                    TextMeshProUGUI[] childrenTexts = report.GetComponentsInChildren<TextMeshProUGUI>();
                    childrenTexts[1].text = text[1];
                    if (text[2] != null)
                        childrenTexts[2].text = text[2];
                    else
                        childrenTexts[2].text = "";
                }

            }

        }


        private async void UpdateIcpPMCanvases(ItemSearchResponse<DublicCoreVocabulary> response)
        {
            foreach (var report in icpPMReportsCollection)
            {
                foreach (var item in response.Items)
                {
                    if (int.Parse(report.name) == item.Id.Id)
                    {
                        icpPMText[0] = (item.Id.Id).ToString();
                        icpPMText[1] = item.Title;
                        icpPMText[2] = item.Vocabulary.DCTermsDescription[0].Value + "\n" + item.Vocabulary.DCTermsDescription[1].Value;
                        icpPMImage = await SearchMedia(item.Id.Id, 0);
                        if (icpPMImage != null) UpdateSingleicpPMCanvas(icpPMText, icpPMImage);
                    }

                }
            }
        }

        [ClientRpc]
        private void UpdateSingleicpPMCanvas(string[] text, byte[] image)
        {
            foreach (var report in icpPMReportsCollection)
            {
                if (report.name == text[0])
                {
                    TextMeshProUGUI[] childrenTexts = report.GetComponentsInChildren<TextMeshProUGUI>();
                    RawImage[] rawImages = report.GetComponentsInChildren<RawImage>();

                    Texture2D tex = new Texture2D(2, 2);
                    // Decode.
                    tex.LoadImage(image);
                    rawImages[0].texture = tex;

                    childrenTexts[1].text = text[1];
                    if (text[2] != null)
                        childrenTexts[2].text = text[2];
                    else
                        childrenTexts[2].text = "";
                }

            }

        }

        private async void UpdateDRCanvases(ItemSearchResponse<DublicCoreVocabulary> response)
        {
            foreach (var report in drReportsCollection)
            {
                foreach (var item in response.Items)
                {
                    if (int.Parse(report.name) == item.Id.Id)
                    {
                        drText[0] = (item.Id.Id).ToString();
                        if (item.Vocabulary.DCTermsDescription.Length > 1)
                            drText[1] = item.Vocabulary.DCTermsDescription[0].Value + "\n \n" + item.Vocabulary.DCTermsDescription[1].Value;
                        else
                            drText[1] = item.Vocabulary.DCTermsDescription[0].Value + "\n \n JPN MISSING";

                        if (item.Vocabulary.ArchaeomHasInterpretationDetails.Length > 1)
                            drText[2] = item.Vocabulary.ArchaeomHasInterpretationDetails[0].Value + "\n \n" + item.Vocabulary.ArchaeomHasInterpretationDetails[1].Value;
                        else
                            drText[2] = item.Vocabulary.ArchaeomHasInterpretationDetails[0].Value + "\n \n JPN MISSING";
                        drImage = await SearchMedia(item.Id.Id, 0);
                        if (drImage != null) UpdateSingleDRCanvas(drText, drImage);
                    }

                }
            }
        }

        [ClientRpc]
        private void UpdateSingleDRCanvas(string[] text, byte[] image)
        {
            foreach (var report in drReportsCollection)
            {
                if (report.name == text[0])
                {
                    TextMeshProUGUI[] childrenTexts = report.GetComponentsInChildren<TextMeshProUGUI>();
                    RawImage[] rawImages = report.GetComponentsInChildren<RawImage>();

                    Texture2D tex = new Texture2D(2, 2);
                    // Decode.
                    tex.LoadImage(image);
                    rawImages[0].texture = tex;

                    childrenTexts[1].text = text[1];
                    if (text[2] != null)
                        childrenTexts[2].text = text[2];
                    else
                        childrenTexts[2].text = "";
                }

            }

        }


        private void UpdateIcpCanvases(ItemSearchResponse<DublicCoreVocabulary> response)
        {
            foreach (var report in icpReportsCollection)
            {
                foreach (var item in response.Items)
                {
                    if (int.Parse(report.name) == item.Id.Id)
                    {
                        icpText[0] = (item.Id.Id).ToString();
                        icpText[1] = item.Title;
                        icpText[2] = item.Vocabulary.DCTermsDescription[0].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationDetails[0].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationTerm[0].Value;
                        if (item.Vocabulary.DCTermsDescription.Length > 1)
                        {
                            icpText[2] = icpText[2] + "\n \n" + item.Vocabulary.DCTermsDescription[1].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationDetails[1].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationTerm[1].Value;
                        }
                        UpdateSingleIcpCanvas(icpText);
                    }

                }
            }
        }

        [ClientRpc]
        private void UpdateSingleIcpCanvas(string[] text)
        {
            foreach (var report in icpReportsCollection)
            {
                if (report.name == text[0])
                {
                    TextMeshProUGUI[] childrenTexts = report.GetComponentsInChildren<TextMeshProUGUI>();

                    childrenTexts[1].text = text[1];
                    if (text[2] != null)
                        childrenTexts[2].text = text[2];
                    else
                        childrenTexts[2].text = "";
                }

            }

        }

        private void UpdatePLMCanvases(ItemSearchResponse<DublicCoreVocabulary> response)
        {
            foreach (var report in plmReportsCollection)
            {
                foreach (var item in response.Items)
                {
                    if (int.Parse(report.name) == item.Id.Id)
                    {
                        plmText[0] = (item.Id.Id).ToString();
                        plmText[1] = item.Vocabulary.DCTermsDescription[0].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationDetails[0].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationTerm[0].Value;
                        if (item.Vocabulary.DCTermsDescription.Length > 1)
                        {
                            plmText[1] = plmText[1] + "\n \n" + item.Vocabulary.DCTermsDescription[1].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationDetails[1].Value + " \n" + item.Vocabulary.ArchaeomHasInterpretationTerm[1].Value;
                        }
                        else
                        {
                            plmText[1] = plmText[1] + "\n \n JPN MISSING";
                        }
                        UpdateSinglePLMCanvas(plmText);
                    }

                }
            }
        }

        [ClientRpc]
        private void UpdateSinglePLMCanvas(string[] text)
        {
            foreach (var report in plmReportsCollection)
            {
                if (report.name == text[0])
                {
                    TextMeshProUGUI[] childrenTexts = report.GetComponentsInChildren<TextMeshProUGUI>();
                    childrenTexts[1].text = text[1];
                }

            }

        }

        private async void UpdateSEMCanvases(ItemSearchResponse<DublicCoreVocabulary> response)
        {
            foreach (var report in semReportsCollection)
            {
                foreach (var item in response.Items)
                {
                    if (int.Parse(report.name) == item.Id.Id)
                    {
                        semText[0] = (item.Id.Id).ToString();
                        semText[1] = item.Title;
                        semText[2] = item.Vocabulary.DCTermsDescription[0].Value + "\n" + item.Vocabulary.DCTermsDescription[1].Value;
                        semImage = await SearchMedia(item.Id.Id, 0);
                        if (semImage != null) UpdateSingleSEMCanvas(semText, semImage);
                    }

                }
            }
        }

        [ClientRpc]
        private void UpdateSingleSEMCanvas(string[] text, byte[] image)
        {
            foreach (var report in semReportsCollection)
            {
                if (report.name == text[0])
                {
                    TextMeshProUGUI[] childrenTexts = report.GetComponentsInChildren<TextMeshProUGUI>();
                    RawImage[] rawImages = report.GetComponentsInChildren<RawImage>();

                    Texture2D tex = new Texture2D(2, 2);
                    // Decode.
                    tex.LoadImage(image);
                    rawImages[0].texture = tex;

                    childrenTexts[1].text = text[1];
                    if (text[2] != null)
                        childrenTexts[2].text = text[2];
                    else
                        childrenTexts[2].text = "";
                }

            }

        }

        private async void UpdateOMforSEMCanvases(ItemSearchResponse<DublicCoreVocabulary> response)
        {
            foreach (var report in omForSemReportsCollection)
            {
                foreach (var item in response.Items)
                {
                    if (int.Parse(report.name) == item.Id.Id)
                    {
                        omForSemText[0] = (item.Id.Id).ToString();
                        omForSemText[1] = item.Title;
                        omForSemText[2] = item.Vocabulary.DCTermsDescription[0].Value + "\n" + item.Vocabulary.DCTermsDescription[1].Value;
                        omForSemImage = await SearchMedia(item.Id.Id, 0);
                        if (omForSemImage != null) UpdateSingleOMforSEMCanvas(omForSemText, omForSemImage);
                    }
                }
            }
        }

        [ClientRpc]
        private void UpdateSingleOMforSEMCanvas(string[] text, byte[] image)
        {
            foreach (var report in omForSemReportsCollection)
            {
                if (report.name == text[0])
                {
                    TextMeshProUGUI[] childrenTexts = report.GetComponentsInChildren<TextMeshProUGUI>();
                    RawImage[] rawImages = report.GetComponentsInChildren<RawImage>();

                    Texture2D tex = new Texture2D(2, 2);
                    // Decode.
                    tex.LoadImage(image);
                    rawImages[0].texture = tex;

                    childrenTexts[1].text = text[1];
                    if (text[2] != null)
                        childrenTexts[2].text = text[2];
                    else
                        childrenTexts[2].text = "";
                }

            }

        }


        public async Task<byte[]> SearchMedia(int index, int mediaIndex)//, RawImage imageContainer)
        {
            MediaSearchResponse<DublicCoreVocabulary> response = await Api.SearchMedia(new MediaSearchParams() { item_id = index });
            byte[] image = await ShowMedia(response.Media[mediaIndex].OriginalUrl); //, imageContainer);
            return image;
        }

        private async Task<byte[]> ShowMedia(string url)//, RawImage container)
        {
            var request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url);

            await request.SendWebRequest();

            if (!request.isNetworkError && !request.isHttpError)
            {
                processedImages++;
                Texture2D tex = UnityEngine.Networking.DownloadHandlerTexture.GetContent(request);
                // Encode.
                byte[] bytes = tex.EncodeToJPG();
                return bytes;
            }
            else
            {
                processedImages = -1;
                return null;
            }
        }
    }
}

