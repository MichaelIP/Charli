using McpNetwork.Charli.Server.Models.Enums;
using McpNetwork.Charli.Server.Models.Interfaces.Managers;
using McpNetwork.Charli.Service.Environment.Interfaces.Managers.Master;
using Microsoft.Extensions.DependencyInjection;
using Pv;
using System.Globalization;
using System.Runtime.InteropServices;

namespace McpNetwork.Charli.Server.Managers.VoiceTool
{
    public class VoiceToolsManager : ACharliManager, IVoiceToolsManager
    {

        private const string voicesPath = @"Managers\VoiceTool\Voices";

        private enum EVoiceGender
        {
            male,
            female,
        }

        private EVoiceGender voiceGender;
        private string? currentLanguage = null; 
        private string? picoVoiceAccessKey;
        
        private Orca? orca;
        private PvSpeaker? speaker;


        #region interface implementation

        public override string Name => "Voice Tools Manager";

        public override string Information => "Provides voice facilities";

        public override EManagerStatus Status => GetStatus();

        public override EManagersType Code => EManagersType.VoiceToolsManager;

        public override Version Version => typeof(VoiceToolsManager).Assembly.GetName().Version ?? new Version("0.0.0");

        public override string MobileEndPoint => "Not implemented";

        public override string MobileIcon => "Not implemented";


        private EManagerStatus GetStatus()
        {
            return EManagerStatus.Running;
        }

        #endregion

        public override bool Initialize(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetService<ISettingManager>();

            picoVoiceAccessKey = settings?.GetSetting<string>("VoiceManager", "PicoVoice.AccessKey") ?? string.Empty;
            if (!Enum.TryParse<EVoiceGender>(settings?.GetSetting<string>("VoiceManager", "VoiceGender"), true, out voiceGender))
            {
                voiceGender = EVoiceGender.female; // Default
            }
            return true;
        }

        public override void Start()
        {
            orca = Orca.Create(picoVoiceAccessKey, currentLanguage);
            
            speaker = new PvSpeaker(orca.SampleRate, 16);
            speaker.Start();
            
            base.Start();
        }


        public override void Stop()
        {
            speaker?.Stop();
            speaker?.Dispose();

            orca?.Dispose();

            base.Stop();
        }


        public void SetLanguage(string locale)
        {
            var language = new CultureInfo(locale);
            if (language.TwoLetterISOLanguageName != currentLanguage) 
            {
                currentLanguage = Path.Combine(voicesPath, string.Format("orca_params_{0}_{1}.pv", language.TwoLetterISOLanguageName, voiceGender.ToString()));
                Stop();
                Start();
            }
        }

        public void Speak(string sentence)
        {
            var result = orca?.Synthesize(sentence);
            var bytes = MemoryMarshal.AsBytes(result.Pcm.AsSpan());
            speaker?.Write(bytes.ToArray());
        }
    }
}
