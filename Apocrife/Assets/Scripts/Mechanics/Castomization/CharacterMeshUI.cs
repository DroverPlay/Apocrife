using Unity.VisualScripting;
using UnityEngine;

public class CharacterMeshUI : MonoBehaviour
{
    public UniversalCharacterMeshCustomizer customizer;

    public void SetMale() => customizer.SetMale();
    public void SetFemale() => customizer.SetFemale();

    public void HairNext() => customizer.Next(CharacterPartCategory.Hair);
    public void HairPrev() => customizer.Previous(CharacterPartCategory.Hair);

    public void FacialHairNext() => customizer.Next(CharacterPartCategory.FacialHair);
    public void FacialHairPrev() => customizer.Previous(CharacterPartCategory.FacialHair);

    public void HeadNext() => customizer.Next(CharacterPartCategory.Head);
    public void HeadPrev() => customizer.Previous(CharacterPartCategory.Head);

    public void TorsoNext() => customizer.Next(CharacterPartCategory.Torso);
    public void TorsoPrev() => customizer.Previous(CharacterPartCategory.Torso);

    //Начало рук
    private void leftArmNext() => customizer.Next(CharacterPartCategory.LeftArm);
    private void leftArmPrev() => customizer.Previous(CharacterPartCategory.LeftArm);

    private void rightArmNext() => customizer.Next(CharacterPartCategory.RightArm);
    private void rightArmPrev() => customizer.Previous(CharacterPartCategory.RightArm);

    private void leftForearmNext() => customizer.Next(CharacterPartCategory.LeftForearm);
    private void leftForearmPrev() => customizer.Previous(CharacterPartCategory.LeftForearm);

    private void rightForearmNext() => customizer.Next(CharacterPartCategory.RightForearm);
    private void rightForearmPrev() => customizer.Previous(CharacterPartCategory.RightForearm);

    private void leftHandNext() => customizer.Next(CharacterPartCategory.LeftHand);
    private void leftHandPrev() => customizer.Previous(CharacterPartCategory.LeftHand);

    private void rightHandNext() => customizer.Next(CharacterPartCategory.RightHand);
    private void rightHandPrev() => customizer.Previous(CharacterPartCategory.RightHand);
    //Конец рук

    //Начало ног
    private void legsNext() => customizer.Next(CharacterPartCategory.Legs);
    private void legsPrev() => customizer.Previous(CharacterPartCategory.Legs);

    private void leftCalfNext() => customizer.Next(CharacterPartCategory.LeftCalf);
    private void leftCalfPrev() => customizer.Previous(CharacterPartCategory.LeftCalf);

    private void rightCalfNext() => customizer.Next(CharacterPartCategory.RightCalf);
    private void rightCalfPrev() => customizer.Previous(CharacterPartCategory.RightCalf);

    private void leftFeetNext() => customizer.Next(CharacterPartCategory.LeftFeet);
    private void leftFeetPrev() => customizer.Previous(CharacterPartCategory.LeftFeet);

    private void rightFeetNext() => customizer.Next(CharacterPartCategory.RightFeet);
    private void rightFeetPrev() => customizer.Previous(CharacterPartCategory.RightFeet);
    //Конец ног


    public void CapeNext() => customizer.Next(CharacterPartCategory.Cape);
    public void CapePrev() => customizer.Previous(CharacterPartCategory.Cape);

    public void BeltNext() => customizer.Next(CharacterPartCategory.Belt);
    public void BeltPrev() => customizer.Previous(CharacterPartCategory.Belt);

    private void leftElbowsNext() => customizer.Next(CharacterPartCategory.LeftElbows);
    private void leftElbowsPrev() => customizer.Previous(CharacterPartCategory.LeftElbows);

    private void rightElbowsNext() => customizer.Next(CharacterPartCategory.RightElbows);
    private void rightElbowsPrev() => customizer.Previous(CharacterPartCategory.RightElbows);

    public void EyebrowsNext() => customizer.Next(CharacterPartCategory.Eyebrows);
    public void EyebrowsPrev() => customizer.Previous(CharacterPartCategory.Eyebrows);

    private void leftKneeNext() => customizer.Next(CharacterPartCategory.LeftKnee);
    private void leftKneePrev() => customizer.Previous(CharacterPartCategory.LeftKnee);

    private void rightKneeNext() => customizer.Next(CharacterPartCategory.RightKnee);
    private void rightKneePrev() => customizer.Previous(CharacterPartCategory.RightKnee);

    private void leftPauldronNext() => customizer.Next(CharacterPartCategory.LeftPauldron);
    private void leftPauldronPrev() => customizer.Previous(CharacterPartCategory.LeftPauldron);

    private void rightPauldronNext() => customizer.Next(CharacterPartCategory.RightPauldron);
    private void rightPauldronPrev() => customizer.Previous(CharacterPartCategory.RightPauldron);

    public void Randomize() => customizer.Randomize();


    //Руки
    public void FullHandNext()
    {
        leftArmNext();
        rightArmNext();
        leftForearmNext();
        rightForearmNext();
        leftHandNext();
        rightHandNext();
    }
    public void FullHandPrev()
    {
        leftArmPrev();
        rightArmPrev();
        leftForearmPrev();
        rightForearmPrev();
        leftHandPrev();
        rightHandPrev();
    }

    //Ноги
    public void FullFeetNext()
    {
        legsNext();
        leftCalfNext();
        rightCalfNext();
        leftFeetNext();
        rightFeetNext();
    }
    public void FullFeetPrev()
    {
        legsPrev();
        leftCalfPrev();
        rightCalfPrev();
        leftFeetPrev();
        rightFeetPrev();
    }

    public void ElbowsNext()
    {
        leftElbowsNext();
        rightElbowsNext();
    }

    public void ElbowsPrev()
    {
        leftElbowsPrev();
        rightElbowsPrev();
    }

    public void KneeNext()
    {
        leftKneeNext();
        rightKneeNext();
    }

    public void KneePrev()
    {
        leftKneePrev();
        rightKneePrev();
    }

    public void PauldronNext()
    {
        leftPauldronNext();
        rightPauldronNext();
    }

    public void PauldronPrev() 
    {
        leftPauldronPrev();
        rightPauldronPrev();
    }
}