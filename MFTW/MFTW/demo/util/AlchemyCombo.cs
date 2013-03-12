using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeInwork.FeInwork.util
{
    public class AlchemyCombo
    {
        private int maxLevel;

        private List<ElementType> comboList;
        public List<ElementType> ComboList
        {
            get { return comboList; }
            set { comboList = value; }
        }

        private StringBuilder alchemyId;
        public StringBuilder AlchemyId
        {
            get { return this.alchemyId; }
        }

        private int earthCounter;
        public int EarthCounter
        {
            get { return earthCounter; }
        }

        private int windCounter;
        public int WindCounter
        {
            get { return windCounter; }
        }

        private int fireCounter;
        public int FireCounter
        {
            get { return fireCounter; }
        }

        private int waterCounter;
        public int WaterCounter
        {
            get { return waterCounter; }
        }

        public int Level
        {
            get { return this.alchemyId.Length; }
        }

        /// <summary>
        /// Crea un combo de alquimia con un nivel(capacidad) maximo
        /// </summary>
        /// <param name="maxLevel">Capacidad de elementos a contener</param>
        public AlchemyCombo(int maxLevel)
        {
            this.maxLevel = maxLevel;
            comboList = new List<ElementType>(this.maxLevel);
            alchemyId = new StringBuilder();
        }

        /// <summary>
        /// Agrega un elemento nuevo de alquimia a la lista
        /// </summary>
        /// <param name="element"></param>
        public void Add(ElementType element)
        {
            if (Level <= maxLevel)
            {
                comboList.Add(element);
                updateCounters();
            }
        }

        /// <summary>
        /// Remueve el ultimo elemento insertado en la lista
        /// </summary>
        public void RemoveLast()
        {
            if (comboList.Count > 0)
            {
                ElementType elementToRemove = comboList[comboList.Count - 1];
                comboList.RemoveAt(alchemyId.Length - 1);
                updateCounters();
            }
        }

        /// <summary>
        /// Limpia la lista de combo de alquimia
        /// </summary>
        public void Clear()
        {
            comboList.Clear();
            updateCounters();
        }

        /// <summary>
        /// Actualiza contadores de elementos utilizados en el combo
        /// de alquimia
        /// </summary>
        private void updateCounters()
        {
            earthCounter = 0;
            fireCounter = 0;
            waterCounter = 0;
            windCounter = 0;
            alchemyId.Remove(0, alchemyId.Length);

            for (int i = 0; i < comboList.Count; i++)
            {
                if (comboList[i] == ElementType.FIRE)
                    fireCounter++;
                else if (comboList[i] == ElementType.WIND)
                    windCounter++;
                else if (comboList[i] == ElementType.WATER)
                    waterCounter++;
                else if (comboList[i] == ElementType.EARTH)
                    earthCounter++;

                alchemyId.Append(((int)comboList[i]).ToString());
            }
        }
    }
}
