import React from 'react';
import { Skill } from '../types/game';

interface SkillBarProps {
  skills: Skill[];
  onUseSkill: (skillId: string) => void;
  isActive: boolean;
}

const SkillBar: React.FC<SkillBarProps> = ({ skills, onUseSkill, isActive }) => {
  const getSkillTypeColor = (type: string) => {
    switch (type) {
      case 'attack': return 'from-red-600 to-red-800';
      case 'heal': return 'from-green-600 to-green-800';
      case 'buff': return 'from-blue-600 to-blue-800';
      case 'debuff': return 'from-purple-600 to-purple-800';
      default: return 'from-gray-600 to-gray-800';
    }
  };

  return (
    <div className="flex justify-center space-x-2 my-4">
      {skills.map((skill) => (
        <button
          key={skill.id}
          onClick={() => isActive && skill.currentCooldown === 0 && onUseSkill(skill.id)}
          disabled={!isActive || skill.currentCooldown > 0}
          className={`relative w-12 h-12 rounded-lg flex items-center justify-center 
            bg-gradient-to-b ${getSkillTypeColor(skill.type)}
            ${!isActive || skill.currentCooldown > 0 ? 'opacity-50 cursor-not-allowed' : 'hover:shadow-md hover:scale-105 transition-transform'}
          `}
          title={`${skill.name}: ${skill.description}${skill.damage ? ` (Damage: ${skill.damage})` : ''}${skill.healing ? ` (Healing: ${skill.healing})` : ''}`}
        >
          <span className="text-white text-xs font-bold">{skill.name.substring(0, 4)}</span>
          
          {skill.currentCooldown > 0 && (
            <div className="absolute inset-0 bg-black bg-opacity-60 rounded-lg flex items-center justify-center">
              <span className="text-white text-lg font-bold">{skill.currentCooldown}</span>
            </div>
          )}
        </button>
      ))}
    </div>
  );
};

export default SkillBar;