using UnityEngine;
using System.Collections;

/// <summary>
/// 게임에서 사운드를 재생하는 클래스. 기본적으로 시작시 스크립트를 비활성화 시켜 놓고 시작한다.
/// Game -> Music
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour {

    /// <summary>
    /// 게임에서 재생할 오디오클립을 모아놓은 배열.
    /// Assets -> Kings -> audio -> Retro Mystic 할당
    /// </summary>
	[Tooltip("게임에서 재생할 오디오클립을 할당하세요.")]
	public AudioClip[] _audioClips;

    /// <summary>
    /// 사운드를 재생하기 위한 오디오소스 컨포넌트. 나 자신에 붙어 있는 'Audio Source'컨포넌트를 할당한다.
    /// Game -> Music 할당
    /// </summary>
	[Tooltip("오디오클립을 재생하기 위한 Audiosource 컨퍼넌트를 할당하세요.")]
	public AudioSource mainAudio;

    /// <summary>
    /// 현재 가지고 있는 오디오클립에서 랜점으로 사운드를 재생할건지에 대한 옵션.
    /// 스크립트에서는 false로 시작하나, 인스펙터에서 체크한 상태이므로 랜덤 사운드 재생상태로 시작하다고 생각하면 된다.
    /// </summary>
	[Tooltip("랜덤으로 오디오클립을 선택해서 재생할지 여부. 즉 랜덤 사운드 재생")]
	public bool playRandomAtStart = false;

    /// <summary>
    /// 사운드를 계속 반복한건지에 대한 여부. 인스펙터에서 체크되어 있으므로 사운드가 계속 반복 재생된다고 생각하면 된다.
    /// </summary>
    [Tooltip("선택한 오디오 클립을 반복하거나, 목록의 모든 오디오클립을 재생합니다.")]
	public bool loopSong = false;

    /// <summary>
    /// 현재 등록된 오디오클립 개수.
    /// </summary>
	private int nrOfSources;

    /// <summary>
    /// 현재 재생할 오디오 클립.
    /// </summary>
	private AudioClip  actualSource;

    /// <summary>
    /// 현재 재생할 오디오클립 인덱스 번호.
    /// </summary>
	private int actualIndex = 0;

    /// <summary>
    /// 다음노래로 넘어가는 기능을 테스트하기 위해 만든 변수. 인스펙터에서 체크하면 다음 노래로 재생된다.
    /// </summary>
	public bool testNext = false;

    /// <summary>
    /// 현재 남은 노래 길이. 처음엔 오디오클립의 전체 재생시간을 담아두고 게임플레이 시간이 지나면서 남은 재생시간을 담게 된다.
    /// </summary>
	private float songDuration = 0f;

    /// <summary>
    /// 이 값보다 짧으면 다음 음악으로 넘어간다.
    /// </summary>
	[HideInInspector] /// 인스펙터에서 변수를 숨기는 어트리뷰트. 보아하니 맨처음에는 노출시켰다가 개발과정에서 변수 숨긴듯.
    [Range(0.0f, 5f)] /// 
    [SerializeField] /// 인스펙터에 private 변수를 노출시키는 어트리뷰트
    private float fadeDuration = 0f; 

	void Start () {

        /// 현재 보유한 오디오클립 개수를 파악한다.
		nrOfSources = _audioClips.Length;
        
        /// 랜덤 재생이 체크되어 있으면, 랜덤으로 재생할 오디오클립을 고르고, 아니면 0번 오디오클립을 선택한다.
		if (playRandomAtStart == true)
        {
            /// 랜덤으로 숫자를 골라서, 재생할 오디오클립을 숫자를 배정한다.
			int rnd = Random.Range (0, nrOfSources);
			actualIndex = rnd;
		}
        else /// 랜덤 재생이 비활성화되어 있으면
        {
            /// 무조건 0번째 오디오클립이 재생될 수 있게 숫자를 배정한다.
			actualIndex = 0;
		}

        /// 현재 재생할 오디오클립에 선정된 오디오클립을 할당한다.
		actualSource = _audioClips [actualIndex];

        /// 오디오소스 컴포넌트에 재생할 오디오클립을 할당한다.
		mainAudio.clip = _audioClips [actualIndex];

        /// ??? 코루틴을 실행
		StartCoroutine (multimediaTimer ());

        /// 현재 재생중인 오디오클립의 길이(재생시간)를 초로 환산해서 할당
		songDuration = mainAudio.clip.length;

        /// 사운드를 재생한다.
        mainAudio.Play ();

        /// 반복재생이 체크되어 있으면, 
		if (loopSong == true)
        {
            /// 오디오소스 컨포넌트에서도 반복재생될수 있게 해놓는다.
			mainAudio.loop = true;
		}
	}

    /// <summary>
    /// ??????
    /// </summary>
    /// <returns></returns>
	IEnumerator multimediaTimer()
    {
        /// 오디오 재생시간을 계산하기 위해 로컬변수에 초기값을 세팅.	
		float now = Time.realtimeSinceStartup;
		float last = now;

		while (true)
        {
            /// 게임 시작된 후 실시간으로 경과된 시간(초)를 할당.
			now = Time.realtimeSinceStartup;

            /// 현재 재생중인 오디오클립 재생한 시간을 할당한다. 
            songDuration -= (now - last);

            /// 만약 사운드를 반복재생하지 않으면
			if(loopSong == false)
            {
                /// 설정한 시간보다 노래 남은 시간이 짧은 경우 다음노래(또는 반복재생)로 넘어간다.
                if(songDuration < fadeDuration)
                {
                    nextListedSong();
                    
                    /// 오디오클립 자체가 설정한 시간보다 짧으면 오작동할수 있으니 로그 메시지뜨게 하자.
                    if (songDuration <= fadeDuration)
                    {
                        Debug.LogWarning("오디오 클립이 페이드 시간보다 짧으면 이상한 사운드 동작이 발생할 수 있습니다.");
                    }
			}
			}
            else
            {
				//no pre-time for fading out, if the song replays. This can't be controlled with the volume-Slider.
				if(songDuration<0f){
					//nextListedSong();
										
					//if(songDuration <=fadeDuration){
					//	Debug.LogWarning("An audioclip  is shorter than the fading time, this can cause strange sound behavior.");
					//}
				}
			}
		
			last = now;

			yield return new WaitForSeconds(0.1f); //this is not accurate. 
		}

	}

    /// <summary>
    /// 다음 노래로 넘어가게 하는 메서드.
    /// </summary>
	public void nextSong()
    {
        /// 오디오소스 컨퍼넌트의 루프(반복재생)가 안되도록 체크를 해제하자.
		mainAudio.loop = false;

        /// 다음 사운드가 재생하기 위해서 인덱스번호를 증가시키자.
		actualIndex++;

        /// 만약 인덱스번호가 현재 등록된 오디오클립수보다 많아지면 처음 사운드로 갈수 있게 인덱스번호를 0으로 만들자
		if (actualIndex >= nrOfSources)
        {
			actualIndex = 0;
		}

        /// 현재 재생할 오디오클립에 재생할 오디오클립을 할당한다.
		actualSource = _audioClips [actualIndex];

        /// 오디오소스 컨포넌트에 현재 재생할 오디오클립을 할당한다.
		mainAudio.clip = actualSource;

        /// 오디오를 재생한다.
		mainAudio.Play ();
        
        /// 루프(반복재생)을 다시 활성화시킨다.
		mainAudio.loop = true;

        /// 현재 재생중인 오디오클립의 재생시간을 할당한다.
		songDuration = mainAudio.clip.length;

		loopSong = false;		//the user breaks the loop, we reload this value on next ending song.

	}

    /// <summary>
    /// 현재 선정된 오디오클립을 재생하고, 노래 길이를 저장한다.
    /// </summary>
	private void replaySong()
    {

		actualSource = _audioClips [actualIndex];
		mainAudio.clip = actualSource;
		mainAudio.Play ();
		songDuration = mainAudio.clip.length;
	}

    /// <summary>
    /// 만약 사운드 반복재생이 체크되어 있으면, 계속 같은 노래를 틀고, 아니면 다음 노래를 재생한다.
    /// </summary>
	private void nextListedSong()
    {
        /// 만약 사운드 반복재생에 체크되어 있으면, 계속 같은 노래를 틀고, 아니면 다음 노래를 재생한다. 
		if (loopSong == true)
        {
			replaySong();
		}
        else
        {
			nextSong();
		}
	}
	
	void Update () {

        /// 사용자가 인스펙터에서 다음 노래 재생을 체크하면 다음 노래를 재생한다.
		if (testNext == true)
        {
			nextSong();

            /// 다시 체크가능하도록 체크를 비활성화한다.
			testNext = false;
		}
	}

    /// <summary>
    /// 게임오브젝트 또는 스크립트가 비활성화되는 순간마다, 한번씩 실행한다.
    /// 여기서는 게임이 종료되거나 게임플레이가 중단되었을때 실행된다.
    /// </summary>
	void OnDisable()
    {
        /// 음악재생을 멈추고, 모든 코루틴을 중단한다.
		mainAudio.Stop ();
		StopAllCoroutines ();

	}

    /// <summary>
    /// 게임오브젝트 또는 스크립트가 활성화되는 순간마다. 한번씩 실행한다.
    /// </summary>
	void OnEnable()
    {
        /// 재생할 오디오 클립이 있다면
		if (actualSource != null)
        {
            /// 현재 오디오소스 컨퍼넌트에 할당된 오디오클립 재생시간을 할당한다.
			songDuration = mainAudio.clip.length;   //memorize the song-duration
            /// 사운드를 재생한다.
            mainAudio.Play ();

			StartCoroutine (multimediaTimer ());
		}
	}
}
